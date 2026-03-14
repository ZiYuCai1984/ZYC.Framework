using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core;

public static class UriFormatter
{
    public static Uri Format<T>(
        Uri baseUri,
        T value,
        bool writeDefaultValues = false) where T : notnull
    {
        return Format(baseUri, value, typeof(T), writeDefaultValues);
    }

    public static Uri Format<T>(
        string baseUri,
        T value,
        bool writeDefaultValues = false) where T : notnull
    {
        if (baseUri is null)
        {
            throw new ArgumentNullException(nameof(baseUri));
        }

        return Format(
            new Uri(baseUri, UriKind.RelativeOrAbsolute),
            value,
            typeof(T),
            writeDefaultValues);
    }

    public static Uri Format(
        Uri baseUri,
        object value,
        Type targetType,
        bool writeDefaultValues = false)
    {
        if (baseUri is null)
        {
            throw new ArgumentNullException(nameof(baseUri));
        }

        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (targetType is null)
        {
            throw new ArgumentNullException(nameof(targetType));
        }

        if (!targetType.IsInstanceOfType(value))
        {
            throw new ArgumentException(
                $"Value is not assignable to target type '{targetType.FullName}'.",
                nameof(value));
        }

        var ctor = targetType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor is null)
        {
            throw new InvalidOperationException($"Type '{targetType.FullName}' has no public constructor.");
        }

        var ps = ctor.GetParameters();
        var pairs = new List<KeyValuePair<string, string>>();

        foreach (var p in ps)
        {
            var key = GetQueryKey(p);

            var prop = targetType.GetProperty(
                p.Name ?? string.Empty,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            if (prop is null || !prop.CanRead)
            {
                throw new InvalidOperationException(
                    $"Type '{targetType.FullName}' does not have a readable public property matching constructor parameter '{p.Name}'.");
            }

            object? rawValue;
            try
            {
                rawValue = prop.GetValue(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Failed to read property '{prop.Name}' from type '{targetType.FullName}'.",
                    ex);
            }

            if (ShouldSkipValue(p, rawValue, writeDefaultValues))
            {
                continue;
            }

            try
            {
                AppendParameter(pairs, key, p.ParameterType, rawValue);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new FormatException(
                    $"Failed to format query parameter '{key}' from {targetType.Name}.{prop.Name} ({p.ParameterType.Name}).",
                    ex);
            }
        }

        return BuildUri(baseUri, pairs);
    }

    private static string GetQueryKey(ParameterInfo p)
    {
        var attr = p.GetCustomAttribute<UriQueryNameAttribute>();
        if (attr != null && !string.IsNullOrWhiteSpace(attr.Name))
        {
            return attr.Name;
        }

        return p.Name ?? throw new InvalidOperationException("Constructor parameter has no name.");
    }

    private static bool ShouldSkipValue(
        ParameterInfo p,
        object? value,
        bool writeDefaultValues)
    {
        if (value is null)
        {
            return true;
        }

        if (!writeDefaultValues && p.HasDefaultValue && AreEqual(value, p.DefaultValue))
        {
            return true;
        }

        if (value is string s && string.IsNullOrEmpty(s))
        {
            return true;
        }

        return false;
    }

    private static void AppendParameter(
        List<KeyValuePair<string, string>> pairs,
        string key,
        Type pType,
        object? value)
    {
        if (value is null)
        {
            return;
        }

        var underlyingNullable = Nullable.GetUnderlyingType(pType);
        var effectiveType = underlyingNullable ?? pType;

        if (TryAppendStringList(pairs, key, effectiveType, value))
        {
            return;
        }

        var raw = FormatSingleValue(effectiveType, value);

        if (string.IsNullOrEmpty(raw))
        {
            return;
        }

        pairs.Add(new KeyValuePair<string, string>(key, raw));
    }

    private static bool TryAppendStringList(
        List<KeyValuePair<string, string>> pairs,
        string key,
        Type effectiveType,
        object value)
    {
        var isStringArray = effectiveType == typeof(string[]);
        var isList = effectiveType == typeof(List<string>);
        var isIReadOnlyList = effectiveType == typeof(IReadOnlyList<string>);
        var isIEnumerable = effectiveType == typeof(IEnumerable<string>);

        if (!(isStringArray || isList || isIReadOnlyList || isIEnumerable))
        {
            return false;
        }

        if (value is not IEnumerable<string> items)
        {
            return true;
        }

        foreach (var item in items)
        {
            if (string.IsNullOrEmpty(item))
            {
                continue;
            }

            pairs.Add(new KeyValuePair<string, string>(key, item));
        }

        return true;
    }

    private static string FormatSingleValue(Type effectiveType, object value)
    {
        if (effectiveType == typeof(string))
        {
            return (string)value;
        }

        if (effectiveType == typeof(Uri))
        {
            return value is Uri uri ? uri.OriginalString : value.ToString()!;
        }

        if (effectiveType == typeof(bool))
        {
            return (bool)value ? "true" : "false";
        }

        if (effectiveType.IsEnum)
        {
            return Enum.Format(effectiveType, value, "G");
        }

        if (effectiveType == typeof(int))
        {
            return ((int)value).ToString(CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(long))
        {
            return ((long)value).ToString(CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(double))
        {
            return ((double)value).ToString(CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(Guid))
        {
            return ((Guid)value).ToString("D");
        }

        if (effectiveType == typeof(DateTimeOffset))
        {
            return ((DateTimeOffset)value).ToString("o", CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(DateTime))
        {
            return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
        }

        var converted = TryConvertToStringByTypeConverter(effectiveType, value);
        if (converted.success)
        {
            return converted.value!;
        }

        throw new InvalidOperationException($"Unsupported parameter type: {effectiveType.FullName}.");
    }

    private static (bool success, string? value) TryConvertToStringByTypeConverter(Type t, object value)
    {
        try
        {
            var conv = TypeDescriptor.GetConverter(t);
            if (conv is not null && conv.CanConvertTo(typeof(string)))
            {
                return (true, conv.ConvertToInvariantString(value));
            }
        }
        catch
        {
            // ignore
        }

        return (false, null);
    }

    private static bool AreEqual(object? x, object? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Equals(y);
    }

    private static Uri BuildUri(
        Uri baseUri,
        List<KeyValuePair<string, string>> pairs)
    {
        var uriText = baseUri.OriginalString;

        string fragment;
        var hashIndex = uriText.IndexOf('#');
        if (hashIndex >= 0)
        {
            fragment = uriText[hashIndex..];
            uriText = uriText[..hashIndex];
        }
        else
        {
            fragment = string.Empty;
        }

        var queryIndex = uriText.IndexOf('?');
        if (queryIndex >= 0)
        {
            uriText = uriText[..queryIndex];
        }

        var query = BuildQueryString(pairs);

        var finalText = string.IsNullOrEmpty(query)
            ? uriText + fragment
            : $"{uriText}?{query}{fragment}";

        return new Uri(
            finalText,
            baseUri.IsAbsoluteUri ? UriKind.Absolute : UriKind.RelativeOrAbsolute);
    }

    private static string BuildQueryString(List<KeyValuePair<string, string>> pairs)
    {
        if (pairs.Count == 0)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        for (var i = 0; i < pairs.Count; i++)
        {
            if (i > 0)
            {
                sb.Append('&');
            }

            sb.Append(Uri.EscapeDataString(pairs[i].Key));
            sb.Append('=');
            sb.Append(Uri.EscapeDataString(pairs[i].Value));
        }

        return sb.ToString();
    }
}
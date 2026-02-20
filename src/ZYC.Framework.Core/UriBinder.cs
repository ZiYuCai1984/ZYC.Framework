using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Core;

public static class UriBinder
{
    public static T Bind<T>(Uri uri) where T : notnull
    {
        return (T)Bind(uri, typeof(T));
    }

    public static bool TryBind<T>(Uri uri, out T? result) where T : notnull
    {
        try
        {
            result = Bind<T>(uri);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public static object Bind(Uri uri, Type targetType)
    {
        if (uri is null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (targetType is null)
        {
            throw new ArgumentNullException(nameof(targetType));
        }

        var q = QueryHelpers.ParseQuery(uri.Query);

        var ctor = targetType
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault();

        if (ctor is null)
        {
            throw new InvalidOperationException($"Type '{targetType.FullName}' has no public constructor.");
        }

        var ps = ctor.GetParameters();
        var args = new object?[ps.Length];

        for (var i = 0; i < ps.Length; i++)
        {
            var p = ps[i];
            var key = GetQueryKey(p);
            var pType = p.ParameterType;

            try
            {
                args[i] = BindOneParameter(q, key, p, pType);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new FormatException(
                    $"Failed to bind query parameter '{key}' to {targetType.Name}.{p.Name} ({pType.Name}).",
                    ex);
            }
        }

        return ctor.Invoke(args);
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

    private static object? BindOneParameter(
        Dictionary<string, StringValues> q,
        string key,
        ParameterInfo p,
        Type pType)
    {
        var underlyingNullable = Nullable.GetUnderlyingType(pType);
        var effectiveType = underlyingNullable ?? pType;

        if (TryBindStringList(q, key, effectiveType, out var listObj))
        {
            if (listObj is null)
            {
                return GetDefaultOrNull(p, pType);
            }

            return listObj;
        }

        if (!q.TryGetValue(key, out var sv) || sv.Count == 0 || string.IsNullOrEmpty(sv[0]))
        {
            return GetDefaultOrNull(p, pType);
        }

        var raw = sv[0]!;

        if (effectiveType == typeof(string))
        {
            return raw;
        }

        if (effectiveType == typeof(Uri))
        {
            if (Uri.TryCreate(raw, UriKind.RelativeOrAbsolute, out var u))
            {
                return u;
            }

            throw new FormatException($"Invalid Uri: '{raw}'.");
        }

        if (effectiveType == typeof(bool))
        {
            if (TryParseBool(raw, out var b))
            {
                return b;
            }

            throw new FormatException($"Invalid bool: '{raw}'.");
        }

        if (effectiveType.IsEnum)
        {
            if (Enum.TryParse(effectiveType, raw, true, out var e))
            {
                return e;
            }

            throw new FormatException($"Invalid enum {effectiveType.Name}: '{raw}'.");
        }


        if (effectiveType == typeof(int))
        {
            return int.Parse(raw, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(long))
        {
            return long.Parse(raw, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(double))
        {
            return double.Parse(raw, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(Guid))
        {
            return Guid.Parse(raw);
        }

        if (effectiveType == typeof(DateTimeOffset))
        {
            return DateTimeOffset.Parse(raw, CultureInfo.InvariantCulture);
        }

        if (effectiveType == typeof(DateTime))
        {
            return DateTime.Parse(raw, CultureInfo.InvariantCulture);
        }

        var converted = TryConvertByTypeConverter(effectiveType, raw);
        if (converted.success)
        {
            return converted.value;
        }

        throw new InvalidOperationException($"Unsupported parameter type: {pType.FullName}.");
    }

    private static object? GetDefaultOrNull(ParameterInfo p, Type pType)
    {
        if (p.HasDefaultValue)
        {
            return p.DefaultValue;
        }

        if (!pType.IsValueType || Nullable.GetUnderlyingType(pType) != null)
        {
            return null;
        }

        return Activator.CreateInstance(pType);
    }

    private static bool TryBindStringList(
        Dictionary<string, StringValues> q,
        string key,
        Type effectiveType,
        out object? listObj)
    {
        listObj = null;

        var isStringArray = effectiveType == typeof(string[]);
        var isList = effectiveType == typeof(List<string>);
        var isIReadOnlyList = effectiveType == typeof(IReadOnlyList<string>);
        var isIEnumerable = effectiveType == typeof(IEnumerable<string>);

        if (!(isStringArray || isList || isIReadOnlyList || isIEnumerable))
        {
            return false;
        }

        if (!q.TryGetValue(key, out var sv) || sv.Count == 0)
        {
            listObj = null;
            return true;
        }

        var arr = sv.ToArray();

        if (isStringArray)
        {
            listObj = arr;
            return true;
        }

        var list = arr.ToList();

        if (isList)
        {
            listObj = list;
            return true;
        }

        // IReadOnlyList<string> / IEnumerable<string>
        listObj = list;
        return true;
    }

    private static bool TryParseBool(string raw, out bool b)
    {
        raw = raw.Trim();

        if (raw == "1")
        {
            b = true;
            return true;
        }

        if (raw == "0")
        {
            b = false;
            return true;
        }

        if (bool.TryParse(raw, out b))
        {
            return true;
        }

        if (string.Equals(raw, "yes", StringComparison.OrdinalIgnoreCase))
        {
            b = true;
            return true;
        }

        if (string.Equals(raw, "no", StringComparison.OrdinalIgnoreCase))
        {
            b = false;
            return true;
        }

        if (string.Equals(raw, "on", StringComparison.OrdinalIgnoreCase))
        {
            b = true;
            return true;
        }

        if (string.Equals(raw, "off", StringComparison.OrdinalIgnoreCase))
        {
            b = false;
            return true;
        }

        b = default;
        return false;
    }

    private static (bool success, object? value) TryConvertByTypeConverter(Type t, string raw)
    {
        try
        {
            var conv = TypeDescriptor.GetConverter(t);
            if (conv is not null && conv.CanConvertFrom(typeof(string)))
            {
                return (true, conv.ConvertFromInvariantString(raw));
            }
        }
        catch
        {
            // ignore
        }

        return (false, null);
    }
}
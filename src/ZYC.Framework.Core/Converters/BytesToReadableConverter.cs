namespace ZYC.Framework.Core.Converters;

public sealed class BytesToReadableConverter : ValueConverterBase<object?, string>
{
    private static bool TryToInt64(object value, out long result)
    {
        switch (value)
        {
            case long l:
                result = l;
                return true;
            case int i:
                result = i;
                return true;
            case ulong ul when ul <= long.MaxValue:
                result = (long)ul;
                return true;
            default:
                try
                {
                    result = System.Convert.ToInt64(value);
                    return true;
                }
                catch
                {
                    result = 0;
                    return false;
                }
        }
    }

    protected override string InternalConvert(object? value)
    {
        if (value == null)
        {
            return "—";
        }

        if (!TryToInt64(value, out var bytes) || bytes < 0)
        {
            return "—";
        }

        // ReSharper disable InconsistentNaming
        const double KB = 1024;
        const double MB = KB * 1024;
        const double GB = MB * 1024;

        if (bytes >= GB)
        {
            return $"{bytes / GB:F2} GB";
        }

        if (bytes >= MB)
        {
            return $"{bytes / MB:F1} MB";
        }

        if (bytes >= KB)
        {
            return $"{bytes / KB:F0} KB";
        }

        return $"{bytes} B";
    }

    protected override object InternalConvertBack(string value)
    {
        throw new NotSupportedException();
    }
}
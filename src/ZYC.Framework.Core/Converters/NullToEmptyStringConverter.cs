namespace ZYC.Framework.Core.Converters;

/// <summary>
///     For suppress System.Windows.Data Error: 23 : Cannot convert 'null' from type 'null' to type 'System.String' for
///     'en-US' culture with default conversions; consider using Converter property of Binding.
///     NotSupportedException:'System.NotSupportedException: 'UriTypeConverter' is unable to convert '(null)' to
///     'System.String'. at
/// </summary>
public class NullToEmptyStringConverter : ValueConverterBase<object?, string>
{
    protected override string InternalConvert(object? value)
    {
        return value?.ToString() ?? "";
    }

    protected override object? InternalConvertBack(string value)
    {
        throw new NotImplementedException();
    }
}
using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class EqualsConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value?.Equals(parameter) ?? false;
        return Reverse ^ result;
    }

    public override object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value?.Equals(parameter) ?? false;
        return Reverse ^ result;
    }
}
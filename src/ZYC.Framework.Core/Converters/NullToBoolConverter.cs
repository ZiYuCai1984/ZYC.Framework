using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class NullToBoolConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value != null;
        return Reverse ^ result;
    }
}
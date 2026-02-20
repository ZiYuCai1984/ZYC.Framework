using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class BoolToDoubleConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public double TrueValue { get; set; } = 1.0;

    public double FalseValue { get; set; }

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolean)
        {
            boolean = Reverse ^ boolean;

            if (boolean)
            {
                return TrueValue;
            }

            return FalseValue;
        }

        throw new ArgumentException();
    }
}
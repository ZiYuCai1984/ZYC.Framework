using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class BoolToIntConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public int TrueValue { get; set; } = 1;

    public int FalseValue { get; set; } = 0;

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
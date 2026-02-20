using System.Globalization;
using System.Windows;

namespace ZYC.Framework.Core.Converters;

public class BoolToVisibilityConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public Visibility TrueValue { get; set; } = Visibility.Visible;

    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

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
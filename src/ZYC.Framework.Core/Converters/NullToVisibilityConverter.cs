using System.Globalization;
using System.Windows;

namespace ZYC.Framework.Core.Converters;

public class NullToVisibilityConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public Visibility TrueValue { get; set; } = Visibility.Visible;

    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var result = value != null;
        result = Reverse ^ result;

        if (result)
        {
            return TrueValue;
        }

        return FalseValue;
    }
}
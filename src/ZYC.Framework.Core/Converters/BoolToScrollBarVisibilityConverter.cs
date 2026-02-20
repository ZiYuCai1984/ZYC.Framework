using System.Globalization;
using System.Windows.Controls;

namespace ZYC.Framework.Core.Converters;

public class BoolToScrollBarVisibilityConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public ScrollBarVisibility TrueValue { get; set; } = ScrollBarVisibility.Visible;

    public ScrollBarVisibility FalseValue { get; set; } = ScrollBarVisibility.Disabled;

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
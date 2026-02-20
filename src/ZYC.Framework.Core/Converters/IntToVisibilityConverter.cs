using System.Globalization;
using System.Windows;

namespace ZYC.Framework.Core.Converters;

public class IntToVisibilityConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public Visibility TrueValue { get; set; } = Visibility.Visible;

    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    public override object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!int.TryParse(value?.ToString() ?? "0", out var result))
        {
            throw new ArgumentException();
        }


        if (result != 0)
        {
            if (!Reverse)
            {
                return TrueValue;
            }

            return FalseValue;
        }
        // ReSharper disable once RedundantIfElseBlock
        else
        {
            if (!Reverse)
            {
                return FalseValue;
            }

            return TrueValue;
        }
    }
}
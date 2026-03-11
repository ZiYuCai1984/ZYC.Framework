using System.Globalization;

namespace ZYC.Framework.Core.Converters;

public class BoolToObjectConverter : ConverterBase
{
    public bool Reverse { get; set; }

    public object? TrueValue { get; set; }

    public object? FalseValue { get; set; }

#pragma warning disable CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    public override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
#pragma warning restore CS8764 // Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
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
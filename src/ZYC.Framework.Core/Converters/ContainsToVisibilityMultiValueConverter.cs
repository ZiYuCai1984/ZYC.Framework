using System.Windows;

namespace ZYC.Framework.Core.Converters;

public abstract class ContainsToVisibilityMultiValueConverter<T> : MultiValueConverterBase<T[], T, Visibility>
{
    public Visibility TrueValue { get; set; } = Visibility.Visible;

    public Visibility FalseValue { get; set; } = Visibility.Collapsed;

    public bool Reverse { get; set; }


    protected override Visibility Convert(T[] value1, T value2)
    {
        var result = value1.Contains(value2);
        result ^= Reverse;

        if (result)
        {
            return TrueValue;
        }

        return FalseValue;
    }
}

public class StringContainsToVisibilityMultiValueConverter : ContainsToVisibilityMultiValueConverter<string>
{
}
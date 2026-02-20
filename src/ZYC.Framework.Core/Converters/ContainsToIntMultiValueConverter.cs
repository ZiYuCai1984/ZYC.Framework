namespace ZYC.Framework.Core.Converters;

public abstract class ContainsToIntMultiValueConverter<T> : MultiValueConverterBase<T[], T, int>
{
    public bool Reverse { get; set; }

    public int TrueValue { get; set; } = 1;

    public int FalseValue { get; set; } = 0;

    protected override int Convert(T[] value1, T value2)
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

public class StringContainsToIntMultiValueConverter : ContainsToIntMultiValueConverter<string>
{
}
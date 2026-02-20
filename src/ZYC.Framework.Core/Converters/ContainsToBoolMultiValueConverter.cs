namespace ZYC.Framework.Core.Converters;

public abstract class ContainsToBoolMultiValueConverter<T> : MultiValueConverterBase<T[], T, bool>
{
    public bool Reverse { get; set; }

    protected override bool Convert(T[] value1, T value2)
    {
        var result = value1.Contains(value2);
        return result ^ Reverse;
    }
}

public class StringContainsToBoolMultiValueConverter : ContainsToBoolMultiValueConverter<string>
{
}
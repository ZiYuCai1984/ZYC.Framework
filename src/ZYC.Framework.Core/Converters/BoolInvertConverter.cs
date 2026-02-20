namespace ZYC.Framework.Core.Converters;

public class BoolInvertConverter : ValueConverterBase<bool, bool>
{
    protected override bool InternalConvert(bool value)
    {
        return !value;
    }

    protected override bool InternalConvertBack(bool value)
    {
        return !value;
    }
}
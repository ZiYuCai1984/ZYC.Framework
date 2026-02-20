namespace ZYC.Framework.Core.Converters;

public class EqualsMultiConverter : MultiValueConverterBase<object, object, bool>
{
    public bool Reverse { get; set; }


    protected override bool Convert(object value1, object value2)
    {
        return Reverse ^ value1.Equals(value2);
    }
}
namespace ZYC.Framework.Abstractions.Config.Attributes;

public class DescriptionAttribute : Attribute
{
    public DescriptionAttribute(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
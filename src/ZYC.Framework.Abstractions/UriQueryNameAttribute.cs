namespace ZYC.Framework.Abstractions;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class UriQueryNameAttribute : Attribute
{
    public UriQueryNameAttribute(string name)
    {
        Name = name;
    }

    public string Name { get; }
}
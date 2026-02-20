namespace ZYC.Framework.Abstractions.Tab;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ConstantsSourceAttribute : Attribute
{
    public ConstantsSourceAttribute(Type type)
    {
        Type = type;
    }

    public Type Type { get; }
}
namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Specifies that a class is a source of constants.
///     This attribute is primarily used for metadata identification or reflection-based discovery.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ConstantsSourceAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ConstantsSourceAttribute" /> class.
    /// </summary>
    /// <param name="type">The underlying type that provides the constant values.</param>
    public ConstantsSourceAttribute(Type type)
    {
        Type = type;
    }

    /// <summary>
    ///     Gets the type associated with the constants source.
    /// </summary>
    public Type Type { get; }
}
namespace ZYC.Framework.Abstractions;

/// <summary>
///     Specifies the custom name for a URI query parameter associated with a property or parameter.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public sealed class UriQueryNameAttribute : Attribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UriQueryNameAttribute" /> class.
    /// </summary>
    /// <param name="name">The name of the query string parameter.</param>
    public UriQueryNameAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Gets the custom name of the URI query parameter.
    /// </summary>
    public string Name { get; }
}
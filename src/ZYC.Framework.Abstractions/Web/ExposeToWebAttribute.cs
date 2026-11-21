namespace ZYC.Framework.Abstractions.Web;

/// <summary>
///     Marks an interface whose methods web pages hosted in WebView2 can call through the web bridge.
///     The interface must be registered in the dependency injection container, and only pages whose host
///     handles bridge messages can reach it.
///     The <c>web-types</c> command of the CLI generates TypeScript declarations for marked interfaces.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public class ExposeToWebAttribute : Attribute
{
    /// <summary>
    ///     Gets or sets the service name used by web pages.
    ///     If null, the interface name without its leading <c>I</c> is used.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     Gets the service name that web pages use for the specified interface.
    /// </summary>
    /// <param name="interfaceType">An interface marked with <see cref="ExposeToWebAttribute" />.</param>
    /// <returns>The configured name, or the interface name without its leading <c>I</c>.</returns>
    public static string GetServiceName(Type interfaceType)
    {
        var attribute = (ExposeToWebAttribute?)GetCustomAttribute(interfaceType, typeof(ExposeToWebAttribute));
        var configuredName = attribute?.Name;
        if (!string.IsNullOrWhiteSpace(configuredName))
        {
            return configuredName;
        }

        var name = interfaceType.Name;
        return name.Length >= 2 && name[0] == 'I' && char.IsUpper(name[1]) ? name[1..] : name;
    }
}

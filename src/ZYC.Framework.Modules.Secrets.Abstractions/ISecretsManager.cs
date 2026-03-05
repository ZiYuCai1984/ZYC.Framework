using ZYC.Framework.Abstractions.MCP;

namespace ZYC.Framework.Modules.Secrets.Abstractions;

/// <summary>
///     Manages secrets configurations and navigation.
/// </summary>
[ExposeToMCP]
public interface ISecretsManager
{
    /// <summary>
    ///     Gets all secrets configurations.
    /// </summary>
    /// <returns>The secrets configurations.</returns>
    ISecrets[] GetSecretsConfigs();

    /// <summary>
    ///     Brings the specified secrets configuration into view.
    /// </summary>
    /// <param name="configType">The configuration type.</param>
    [ExposeToMCP(RequiresUIThread = true)]
    void BringIntoView(Type configType);

    /// <summary>
    ///     Brings the specified secrets configuration into view.
    /// </summary>
    /// <typeparam name="T">The configuration type.</typeparam>
    [ExposeToMCP(RequiresUIThread = true)]
    void BringIntoView<T>();

    /// <summary>
    ///     Gets the URI of the secrets page.
    /// </summary>
    /// <returns>The secrets page URI.</returns>
    Uri GetPageUri();
}
namespace ZYC.Framework.Modules.Aspire.Abstractions;

/// <summary>
///     Provides command-line based Aspire extension resources.
/// </summary>
public interface ICommandlineResourcesProvider : IExtensionResourcesProvider
{
    /// <summary>
    ///     Registers a command-line service resource.
    /// </summary>
    /// <param name="options">The options describing the service resource.</param>
    void Register(CommandlineServiceOptions options);
}
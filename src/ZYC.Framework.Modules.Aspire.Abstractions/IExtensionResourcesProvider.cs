namespace ZYC.Framework.Modules.Aspire.Abstractions;

/// <summary>
///     Defines a provider that configures resources for an Aspire extension.
/// </summary>
public interface IExtensionResourcesProvider
{
    /// <summary>
    ///     Configures resources using the provided builder instance.
    /// </summary>
    /// <param name="builder">The builder used to configure resources.</param>
    void ConfigureResources(object builder);
}
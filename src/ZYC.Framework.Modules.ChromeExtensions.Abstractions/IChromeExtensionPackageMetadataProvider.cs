namespace ZYC.Framework.Modules.ChromeExtensions.Abstractions;

/// <summary>
///     Provides Chrome Web Store extension package metadata.
/// </summary>
public interface IChromeExtensionPackageMetadataProvider
{
    /// <summary>
    ///     Gets package metadata for the specified Chrome Web Store extension identifier.
    /// </summary>
    /// <param name="extensionId">The Chrome Web Store extension identifier or detail page URL.</param>
    /// <param name="cancellationToken">A cancellation token used to cancel the operation.</param>
    /// <returns>The package metadata returned by the update service.</returns>
    Task<ChromeExtensionPackageMetadata> GetPackageMetadataAsync(
        string extensionId,
        CancellationToken cancellationToken = default);
}

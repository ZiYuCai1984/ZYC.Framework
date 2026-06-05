namespace ZYC.Framework.Modules.ChromeExtensions.Abstractions;

/// <summary>
///     Manages Chrome Web Store extension packages in the local application package store.
/// </summary>
public interface IChromeExtensionPackageManager
{
    /// <summary>
    ///     Gets all locally installed extension package records.
    /// </summary>
    /// <returns>The locally installed extension packages.</returns>
    IReadOnlyList<ChromeInstalledExtension> GetInstalledExtensions();

    /// <summary>
    ///     Gets a locally installed extension package record by extension identifier.
    /// </summary>
    /// <param name="extensionId">The Chrome Web Store extension identifier or detail page URL.</param>
    /// <returns>The installed extension package record, or <c>null</c> when it is not installed locally.</returns>
    ChromeInstalledExtension? GetInstalledExtension(string extensionId);

    /// <summary>
    ///     Determines whether the specified extension identifier is installed locally.
    /// </summary>
    /// <param name="extensionId">The Chrome Web Store extension identifier or detail page URL.</param>
    /// <returns><c>true</c> if a local package record exists; otherwise, <c>false</c>.</returns>
    bool IsInstalled(string extensionId);

    /// <summary>
    ///     Downloads the latest CRX package, unpacks it, and stores it locally.
    /// </summary>
    /// <param name="extensionId">The Chrome Web Store extension identifier or detail page URL.</param>
    /// <param name="cancellationToken">A cancellation token used to cancel the operation.</param>
    /// <returns>The local extension package record.</returns>
    Task<ChromeInstalledExtension> InstallAsync(
        string extensionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Removes the local package and package record for the specified extension identifier.
    /// </summary>
    /// <param name="extensionId">The Chrome Web Store extension identifier or detail page URL.</param>
    /// <returns><c>true</c> if a package was removed; otherwise, <c>false</c>.</returns>
    /// <remarks>
    ///     This method only removes the local package store content. It does not remove extensions from any WebView2
    ///     profile that may have loaded the unpacked extension directory.
    /// </remarks>
    Task<bool> UninstallAsync(string extensionId);
}

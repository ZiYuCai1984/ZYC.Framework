namespace ZYC.Framework.Modules.WebBrowser.Abstractions.ChromeWebStore;

/// <summary>
///     Describes Chrome Web Store package metadata returned by the update service.
/// </summary>
public sealed class ChromeWebStoreExtensionPackageMetadata
{
    /// <summary>
    ///     Gets or sets the Chrome Web Store extension identifier.
    /// </summary>
    public string ExtensionId { get; set; } = "";

    /// <summary>
    ///     Gets or sets the Chrome Web Store detail page URL.
    /// </summary>
    public string StoreUrl { get; set; } = "";

    /// <summary>
    ///     Gets or sets the application status returned by the update service.
    /// </summary>
    public string? AppStatus { get; set; }

    /// <summary>
    ///     Gets or sets the update check status returned by the update service.
    /// </summary>
    public string? UpdateCheckStatus { get; set; }

    /// <summary>
    ///     Gets or sets the package version returned by the update service.
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    ///     Gets or sets the CRX download URL returned by the update service.
    /// </summary>
    public string? DownloadUrl { get; set; }

    /// <summary>
    ///     Gets or sets the package SHA-256 hash returned by the update service.
    /// </summary>
    public string? HashSha256 { get; set; }

    /// <summary>
    ///     Gets or sets the package fingerprint returned by the update service.
    /// </summary>
    public string? Fingerprint { get; set; }

    /// <summary>
    ///     Gets or sets the package size in bytes returned by the update service.
    /// </summary>
    public long? SizeBytes { get; set; }

    /// <summary>
    ///     Gets a value indicating whether downloadable package metadata is available.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool HasPackage => !string.IsNullOrWhiteSpace(DownloadUrl);
}

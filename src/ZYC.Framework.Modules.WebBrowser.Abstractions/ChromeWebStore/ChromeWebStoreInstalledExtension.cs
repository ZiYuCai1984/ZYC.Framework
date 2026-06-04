namespace ZYC.Framework.Modules.WebBrowser.Abstractions.ChromeWebStore;

/// <summary>
///     Describes a Chrome Web Store extension package installed into the local package store.
/// </summary>
public sealed class ChromeWebStoreInstalledExtension
{
    /// <summary>
    ///     Gets or sets the display name read from the extension manifest.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    ///     Gets the display name, falling back to the extension identifier when no manifest name is available.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public string DisplayName => string.IsNullOrWhiteSpace(Name) ? ExtensionId : Name;

    /// <summary>
    ///     Gets or sets the Chrome Web Store extension identifier.
    /// </summary>
    public string ExtensionId { get; set; } = "";

    /// <summary>
    ///     Gets or sets the Chrome Web Store detail page URL.
    /// </summary>
    public string StoreUrl { get; set; } = "";

    /// <summary>
    ///     Gets or sets the installed extension package version.
    /// </summary>
    public string Version { get; set; } = "";

    /// <summary>
    ///     Gets or sets the CRX package download URL used during installation.
    /// </summary>
    public string DownloadUrl { get; set; } = "";

    /// <summary>
    ///     Gets or sets the local CRX package path.
    /// </summary>
    public string PackagePath { get; set; } = "";

    /// <summary>
    ///     Gets or sets the local unpacked extension folder path for callers that load extensions into WebView2.
    /// </summary>
    public string UnpackedPath { get; set; } = "";

    /// <summary>
    ///     Gets or sets the extension toolbar popup page path read from the extension manifest.
    /// </summary>
    public string PopupPagePath { get; set; } = "";

    /// <summary>
    ///     Gets or sets the extension toolbar popup page URL.
    /// </summary>
    public string PopupPageUrl { get; set; } = "";

    /// <summary>
    ///     Gets a value indicating whether the extension has a toolbar popup page.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool HasPopupPage => !string.IsNullOrWhiteSpace(PopupPageUrl);

    /// <summary>
    ///     Gets or sets the extension options page path read from the extension manifest.
    /// </summary>
    public string OptionsPagePath { get; set; } = "";

    /// <summary>
    ///     Gets or sets the extension options page URL.
    /// </summary>
    public string OptionsPageUrl { get; set; } = "";

    /// <summary>
    ///     Gets a value indicating whether the extension has an options page.
    /// </summary>
    [System.Text.Json.Serialization.JsonIgnore]
    public bool HasOptionsPage => !string.IsNullOrWhiteSpace(OptionsPageUrl);

    /// <summary>
    ///     Gets or sets the local package size in bytes.
    /// </summary>
    public long? SizeBytes { get; set; }

    /// <summary>
    ///     Gets or sets the package SHA-256 hash returned by the update service.
    /// </summary>
    public string? HashSha256 { get; set; }

    /// <summary>
    ///     Gets or sets the package fingerprint returned by the update service.
    /// </summary>
    public string? Fingerprint { get; set; }

    /// <summary>
    ///     Gets or sets the time when the package was installed locally.
    /// </summary>
    public DateTimeOffset InstalledAt { get; set; } = DateTimeOffset.UtcNow;
}

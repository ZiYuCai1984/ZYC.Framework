using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Modules.WebBrowser.Abstractions.ChromeWebStore;

/// <summary>
///     Stores locally managed Chrome Web Store extension packages for the web browser module.
/// </summary>
[Hidden]
public sealed class ChromeWebStoreExtensionConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the locally installed extension package records.
    /// </summary>
    public ChromeWebStoreInstalledExtension[] InstalledExtensions { get; set; } = [];
}

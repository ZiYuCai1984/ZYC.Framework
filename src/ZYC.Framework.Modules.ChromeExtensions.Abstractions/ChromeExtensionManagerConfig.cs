using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Modules.ChromeExtensions.Abstractions;

/// <summary>
///     Stores locally managed Chrome Web Store extension packages for the web browser module.
/// </summary>
[Hidden]
public class ChromeExtensionManagerConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the locally installed extension package records.
    /// </summary>
    public ChromeInstalledExtension[] InstalledExtensions { get; set; } = [];

    /// <summary>
    ///     Gets or sets the Chrome Web Store home URL used by the extension store browser.
    /// </summary>
    public string StoreHomeUri { get; set; } = "https://chromewebstore.google.com/";


    public string UpdateServiceUri { get; set; } = "https://clients2.google.com/service/update2/crx?response=updatecheck&prodversion=120.0.0.0&acceptformat=crx2,crx3&x=id%3D{0}%26uc";
}

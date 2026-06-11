using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser.Dialog;

internal class ManagePluginItem
{
    public ManagePluginItem(
        ChromeInstalledExtension extension,
        bool isLoaded)
    {
        Extension = extension;
        IsLoaded = isLoaded;
    }

    private ChromeInstalledExtension Extension { get; }

    public string DisplayName => Extension.DisplayName;

    public string ExtensionId => Extension.ExtensionId;

    public string UnpackedPath => Extension.UnpackedPath;

    public bool IsLoaded { get; }

    public string StateText => IsLoaded ? "Loaded" : "Available";

    public bool CanAdd => !IsLoaded && !string.IsNullOrWhiteSpace(UnpackedPath);

    public bool CanRemove => IsLoaded;
}
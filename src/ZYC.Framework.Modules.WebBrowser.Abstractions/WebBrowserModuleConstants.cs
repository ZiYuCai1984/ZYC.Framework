using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

#pragma warning disable CS1591

public static class WebBrowserModuleConstants
{
    public const string MenuIcon = "Web";

    public const string MenuTitle = "Web Browser";

    public const string ChromeWebStoreExtensionManagerHost = "web-browser";

    public const string ChromeWebStoreExtensionManagerPath = "chrome-web-store-extensions";

    public const string ChromeWebStoreExtensionManagerTitle = "Chrome Extensions";

    public const string ChromeWebStoreExtensionManagerIcon = "PuzzleOutline";

    public static Uri ChromeWebStoreExtensionManagerUri => UriTools.CreateAppUri(
        ChromeWebStoreExtensionManagerHost,
        ChromeWebStoreExtensionManagerPath);
}

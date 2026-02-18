using ZYC.Automation.Abstractions;

namespace ZYC.Automation.Modules.About.Abstractions;

public static class AboutModuleConstants
{
    public const string Icon = "InformationOutline";

    public const string Host = "about";

    public static string Title => $"About {ProductInfo.ProductName}";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
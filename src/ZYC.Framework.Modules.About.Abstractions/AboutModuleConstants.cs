using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.About.Abstractions;

#pragma warning disable CS1591

public static class AboutModuleConstants
{
    public const string Icon = "InformationOutline";

    public const string Host = "about";

    public static string Title => $"About {ProductInfo.ProductName}";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
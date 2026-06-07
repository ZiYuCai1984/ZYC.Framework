using ZYC.Framework.Abstractions;

#pragma warning disable CS1591


namespace ZYC.Framework.Modules.ChromeExtensions.Abstractions;

public static class ChromeExtensionsModuleConstants
{
    public const string Host = "chromeextensions";

    public const string Title = "Chrome Extensions";

    public const string Icon = "GoogleChrome";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
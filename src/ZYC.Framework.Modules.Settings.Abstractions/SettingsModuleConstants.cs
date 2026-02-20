using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Settings.Abstractions;

public static class SettingsModuleConstants
{
    public const string Icon = "ApplicationCogOutline";

    public const string Host = "settings";

    public const string Title = "ApplicationSettings";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
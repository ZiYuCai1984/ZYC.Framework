using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Secrets.Abstractions;

#pragma warning disable CS1591

public static class SecretsModuleConstants
{
    public const string Icon = "🔑";

    public const string Host = "secrets";

    public const string Title = "Secrets";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
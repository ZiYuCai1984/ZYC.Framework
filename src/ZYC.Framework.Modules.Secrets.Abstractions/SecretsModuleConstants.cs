using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Secrets.Abstractions;

public static class SecretsModuleConstants
{
    public const string Icon = "🔑";

    public const string Host = "secrets";

    public const string Title = "Secrets";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.CLI.Abstractions;

#pragma warning disable CS1591

public static class CLIModuleConstants
{
    public const string Host = "cli";

    public const string DefaultTitle = "CLI";

    public const string Icon = "Console";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
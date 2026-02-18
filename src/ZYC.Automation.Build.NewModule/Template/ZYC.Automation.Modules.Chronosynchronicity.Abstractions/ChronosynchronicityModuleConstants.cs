using ZYC.Automation.Abstractions;

// ReSharper disable once CheckNamespace
namespace ZYC.Automation.Modules.Chronosynchronicity.Abstractions;

public static class ChronosynchronicityModuleConstants
{
    public const string Host = "chronosynchronicity";

    public const string Title = "Chronosynchronicity";

    public const string Icon = "CreationOutline";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
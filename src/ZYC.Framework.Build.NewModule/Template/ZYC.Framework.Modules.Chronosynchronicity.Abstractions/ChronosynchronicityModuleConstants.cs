using ZYC.Framework.Abstractions;

#pragma warning disable CS1591

// ReSharper disable once CheckNamespace
namespace ZYC.Framework.Modules.Chronosynchronicity.Abstractions;

public static class ChronosynchronicityModuleConstants
{
    public const string Host = "chronosynchronicity";

    public const string Title = "Chronosynchronicity";

    public const string Icon = "CreationOutline";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
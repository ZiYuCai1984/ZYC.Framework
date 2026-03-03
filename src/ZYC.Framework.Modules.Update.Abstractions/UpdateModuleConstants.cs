using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Update.Abstractions;

#pragma warning disable CS1591

public static class UpdateModuleConstants
{
    public const string Icon = "Update";

    public const string Host = "update";

    public const string Title = "Update";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
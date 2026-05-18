using ZYC.Framework.Abstractions;

#pragma warning disable CS1591

namespace ZYC.Framework.Modules.__PROJECT_NAME__.Abstractions;

public static class __PROJECT_SHORT_NAME__ModuleConstants
{
    public const string Host = "__PROJECT_HOST__";

    public const string Title = "__PROJECT_SHORT_NAME__";

    public const string Icon = "CreationOutline";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}

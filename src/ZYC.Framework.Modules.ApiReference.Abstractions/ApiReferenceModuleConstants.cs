using ZYC.Framework.Abstractions;

#pragma warning disable CS1591


namespace ZYC.Framework.Modules.ApiReference.Abstractions;

public static class ApiReferenceModuleConstants
{
    public const string Host = "api";

    public const string Title = "Api Reference";

    public const string Icon = "CreationOutline";

    public const string DocFolder = "_doc";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
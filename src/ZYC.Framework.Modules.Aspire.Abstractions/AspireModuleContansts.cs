using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Aspire.Abstractions;

#pragma warning disable CS1591

public static class AspireModuleConstants
{
    public const string Host = "aspire";

    public const string Title = "Aspire";

    public const string Icon = Base64IconResources.Aspire;

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
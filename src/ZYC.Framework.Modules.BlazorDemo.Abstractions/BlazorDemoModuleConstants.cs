using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.BlazorDemo.Abstractions;

#pragma warning disable CS1591

// ReSharper disable once StringLiteralTypo

public static class BlazorDemoModuleConstants
{
    public const string Host = "blazordemo";

    public const string Title = "BlazorDemo";

    public const string Icon = Base64IconResources.Blazor;

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
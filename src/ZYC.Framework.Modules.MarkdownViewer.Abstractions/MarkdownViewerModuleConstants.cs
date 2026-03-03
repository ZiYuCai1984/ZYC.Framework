using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.MarkdownViewer.Abstractions;

#pragma warning disable CS1591

public static class MarkdownViewerModuleConstants
{
    public static string Host => "md";

    public static string Title => "MarkdownViewer";

    public static string Icon => "📄";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
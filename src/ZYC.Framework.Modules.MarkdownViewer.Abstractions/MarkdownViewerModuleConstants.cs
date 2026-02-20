using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.MarkdownViewer.Abstractions;

public static class MarkdownViewerModuleConstants
{
    public static string Host => "md";

    public static string Title => "MarkdownViewer";

    public static string Icon => "📄";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
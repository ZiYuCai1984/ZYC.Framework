using ZYC.Automation.Abstractions;

namespace ZYC.Automation.Modules.Gemini.Abstractions;

public static class GeminiModuleConstants
{
    public const string Host = "gemini";

    public const string Title = "Gemini";

    public const string Icon = Base64IconResources.Gemini;

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
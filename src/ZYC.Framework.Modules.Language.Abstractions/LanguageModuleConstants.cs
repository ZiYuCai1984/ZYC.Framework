using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

#pragma warning disable CS1591

public static class LanguageModuleConstants
{
    public const string DefaultIcon = "FormatTextVariantOutline";

    public const string Host = "lang";

    public const string Title = "Language";

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
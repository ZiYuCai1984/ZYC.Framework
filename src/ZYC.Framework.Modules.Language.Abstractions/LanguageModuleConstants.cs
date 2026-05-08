using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

#pragma warning disable CS1591

public static class LanguageModuleConstants
{
    public const string DefaultIcon = "FormatTextVariantOutline";

    public const string Host = "lang";

    public const string Title = "Language";

    public const string Anchor = "Language";

    public static Uri Uri => UriTools.CreateAppUri(Host);

    public static class LocalizationResources
    {
        // ReSharper disable MemberHidesStaticFromOuterClass
        public const string Host = LanguageModuleConstants.Host;

        public const string Path = "/resources";

        public const string Icon = "TableEdit";

        public const string Title = "Localization Resources";

        public static Uri Uri => UriTools.CreateAppUri(Host, Path);
    }
}

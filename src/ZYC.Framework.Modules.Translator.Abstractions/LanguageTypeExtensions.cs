namespace ZYC.Framework.Modules.Translator.Abstractions;

public static class LanguageTypeExtensions
{
    public static string ToDisplayName(this LanguageType language)
    {
        return language switch
        {
            LanguageType.en => "English",
            LanguageType.ja => "日本語",
            LanguageType.zh => "简体中文",
            LanguageType.zt => "繁體中文",
            LanguageType.ko => "한국어",
            _ => language.ToString()
        };
    }
}
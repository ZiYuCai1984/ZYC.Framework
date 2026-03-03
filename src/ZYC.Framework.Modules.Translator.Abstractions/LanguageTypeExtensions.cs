namespace ZYC.Framework.Modules.Translator.Abstractions;

/// <summary>
///     Provides extension methods for the <see cref="LanguageType" /> enumeration.
/// </summary>
public static class LanguageTypeExtensions
{
    /// <summary>
    ///     Converts a <see cref="LanguageType" /> value to its localized display name.
    /// </summary>
    /// <param name="language">The language type value.</param>
    /// <returns>A string representing the human-readable name of the language.</returns>
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
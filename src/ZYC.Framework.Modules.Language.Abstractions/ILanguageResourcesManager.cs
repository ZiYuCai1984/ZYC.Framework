using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
///     Defines a contract for managing, retrieving, and updating localization resource entries across multiple languages.
/// </summary>
public interface ILanguageResourcesManager
{
    /// <summary>
    ///     Gets editable localization resource entries for the specified language.
    /// </summary>
    /// <param name="languageType">The language to inspect.</param>
    /// <returns>The localization resource entries.</returns>
    LanguageResourceEntry[] GetLanguageResourceEntries(LanguageType languageType);

    /// <summary>
    ///     Updates a localization resource entry.
    /// </summary>
    /// <param name="languageType">The language to update.</param>
    /// <param name="key">The source text key.</param>
    /// <param name="value">The localized value.</param>
    /// <returns>The updated localization resource entry.</returns>
    LanguageResourceEntry UpdateLanguageResourceEntry(LanguageType languageType, string key, string value);
}
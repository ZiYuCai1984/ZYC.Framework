using ZYC.Framework.Abstractions.MCP;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
///     Provides language selection and localization services.
/// </summary>
[ExposeToMCP]
public interface ILanguageManager
{
    /// <summary>
    ///     Gets the current language type.
    /// </summary>
    /// <returns>The current language type.</returns>
    LanguageType GetCurrentLanguageType();

    /// <summary>
    ///     Sets the current language type.
    /// </summary>
    /// <param name="languageType">The language type to set.</param>
    void SetCurrentLanguageType(LanguageType languageType);

    /// <summary>
    ///     Localizes the supplied text based on the current language.
    /// </summary>
    /// <param name="text">The text to localize.</param>
    /// <returns>The localized text.</returns>
    string Localization(string text);

    /// <summary>
    ///     Gets all registered language resources.
    /// </summary>
    /// <returns>The language resource configurations.</returns>
    ILanguageResourcesConfig[] GetAllLanguageResources();
}
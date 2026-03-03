namespace ZYC.Framework.Modules.Translator.Abstractions;

/// <summary>
///     Defines a service for translating text between different languages.
///     Supports both synchronous and asynchronous operations.
/// </summary>
public interface ITranslator
{
    /// <summary>
    ///     Translates text asynchronously from a source language to a target language.
    /// </summary>
    /// <param name="text">The source text to translate.</param>
    /// <param name="from">The source language type.</param>
    /// <param name="to">The target language type.</param>
    /// <returns>A task representing the asynchronous translation operation, containing the translated string.</returns>
    Task<string> TranslateAsync(string text, LanguageType from, LanguageType to);

    /// <summary>
    ///     Translates text synchronously from a source language to a target language.
    /// </summary>
    /// <param name="text">The source text to translate.</param>
    /// <param name="from">The source language type.</param>
    /// <param name="to">The target language type.</param>
    /// <returns>The translated string.</returns>
    string Translate(string text, LanguageType from, LanguageType to);

    /// <summary>
    ///     A helper method to translate text from English to a target language asynchronously.
    /// </summary>
    /// <param name="text">The English text to translate.</param>
    /// <param name="to">The target language type.</param>
    /// <returns>The translated string.</returns>
    Task<string> TranslateFromEnglishAsync(string text, LanguageType to)
    {
        return TranslateAsync(text, LanguageType.en, to);
    }

    /// <summary>
    ///     A helper method to translate text from English to a target language synchronously.
    /// </summary>
    /// <param name="text">The English text to translate.</param>
    /// <param name="to">The target language type.</param>
    /// <returns>The translated string.</returns>
    string TranslateFromEnglish(string text, LanguageType to)
    {
        return Translate(text, LanguageType.en, to);
    }
}
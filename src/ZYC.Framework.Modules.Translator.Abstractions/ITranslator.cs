namespace ZYC.Framework.Modules.Translator.Abstractions;

public interface ITranslator
{
    Task<string> TranslateAsync(string text, LanguageType from, LanguageType to);

    string Translate(string text, LanguageType from, LanguageType to);

    Task<string> TranslateFromEnglishAsync(string text, LanguageType to)
    {
        return TranslateAsync(text, LanguageType.en, to);
    }

    string TranslateFromEnglish(string text, LanguageType to)
    {
        return Translate(text, LanguageType.en, to);
    }
}
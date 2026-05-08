namespace ZYC.Framework.Modules.Language;

internal partial class LanguageManager
{
    public string Localization(string text)
    {
        var languageType = CurrentLanguage;


        if (TryGetValueFromCache(languageType, text, out var r))
        {
            return r;
        }

        if (Translator == null)
        {
            return text;
        }


        var translatedResult = Translator.TranslateFromEnglish(text, languageType);
        if (!string.IsNullOrWhiteSpace(translatedResult)
            && translatedResult != text)
        {
            SaveTranslatedResult(languageType, text, translatedResult);
        }

        return translatedResult;
    }
}
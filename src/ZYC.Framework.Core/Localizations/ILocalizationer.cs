namespace ZYC.Framework.Core.Localizations;

public interface ILocalizationer
{
    string Localization(string text);

    Task<string> LocalizationAsync(string text);
}
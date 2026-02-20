using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.Infrastructure;

[RegisterSingleInstanceAs(typeof(ILocalizationer))]
internal class NullLocalizationer : ILocalizationer
{
    public string Localization(string text)
    {
        return text;
    }

    public Task<string> LocalizationAsync(string text)
    {
        return Task.FromResult(text);
    }
}
using System.Reflection;
using LibreTranslate.Net;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Translator;

[RegisterSingleInstanceAs(typeof(ITranslator))]
internal class Translator : ITranslator
{
    public Translator(IAppLogger<Translator> logger, TranslatorConfig translatorConfig)
    {
        Logger = logger;
        TranslatorConfig = translatorConfig;
        LibreTranslate = new LibreTranslate.Net.LibreTranslate(translatorConfig.Url);


        Activator.CreateInstance(
            typeof(LanguageCode),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            ["zt"],
            null);
    }

    private IAppLogger<Translator> Logger { get; }
    private TranslatorConfig TranslatorConfig { get; }

    private LibreTranslate.Net.LibreTranslate LibreTranslate { get; }


    public async Task<string> TranslateAsync(string text, LanguageType from, LanguageType to)
    {
        if (!TranslatorConfig.IsEnabled)
        {
            return text;
        }

        try
        {
            return await LibreTranslate.TranslateAsync(new Translate
            {
                Source = LanguageCode.FromString(from.ToString()),
                Target = LanguageCode.FromString(to.ToString()),
                Text = text
            });
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return text;
        }
    }

    public string Translate(string text, LanguageType from, LanguageType to)
    {
        //!WARNING This is to be called by the UI thread, so it is written like this to prevent deadlock
        var source = new TaskCompletionSource<string>();
        Task.Run(async () =>
        {
            var result = await TranslateAsync(text, from, to);
            source.SetResult(result);
        });

        return source.Task.Result;
    }
}
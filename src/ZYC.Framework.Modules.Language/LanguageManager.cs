using System.Diagnostics;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Common;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.CoreToolkit.Extensions.Settings;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Core.Localizations;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Language.Abstractions.Event;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language;

[RegisterSingleInstanceAs(typeof(ILanguageManager), typeof(ILocalizationer))]
internal class LanguageManager : ILanguageManager, ILocalizationer
{
    public LanguageManager(
        IEventAggregator eventAggregator,
        IBannerManager bannerManager,
        DefaultLanguageResourcesConfig defaultLanguageResourcesConfig,
        ILifetimeScope lifetimeScope,
        LanguageConfig config)
    {
        EventAggregator = eventAggregator;
        BannerManager = bannerManager;
        DefaultLanguageResourcesConfig = defaultLanguageResourcesConfig;
        LifetimeScope = lifetimeScope;
        Config = config;

        if (LifetimeScope.TryResolve<ITranslator>(out var translator))
        {
            Translator = translator;
        }


        LanguageResources = MergeLanguages(GetAllLanguageResources());
    }

    private IEventAggregator EventAggregator { get; }

    private IBannerManager BannerManager { get; }

    private DefaultLanguageResourcesConfig DefaultLanguageResourcesConfig { get; }

    private ILifetimeScope LifetimeScope { get; }

    private Dictionary<LanguageType, SnapshotDictionary<string, string>> LanguageResources { get; }

    private ITranslator? Translator { get; }

    private LanguageConfig Config { get; }

    private LanguageType CurrentLanguage
    {
        get => Config.CurrentLanguage;
        set => Config.CurrentLanguage = value;
    }

    public LanguageType GetCurrentLanguageType()
    {
        return CurrentLanguage;
    }

    public void SetCurrentLanguageType(LanguageType languageType)
    {
        CurrentLanguage = languageType;
        EventAggregator.Publish(new LanguageChangedEvent());

        BannerManager.PromptRestart();
    }

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

    public ILanguageResourcesConfig[] GetAllLanguageResources()
    {
        var languages = LifetimeScope.Resolve<ILanguageResourcesConfig[]>().ToList();

        languages.Remove(DefaultLanguageResourcesConfig);
        languages.Insert(0, DefaultLanguageResourcesConfig);

        return languages.ToArray();
    }

    public async Task<string> LocalizationAsync(string text)
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

        var translatedResult = await Translator.TranslateFromEnglishAsync(text, languageType);
        if (!string.IsNullOrWhiteSpace(translatedResult)
            && translatedResult != text)
        {
            SaveTranslatedResult(languageType, text, translatedResult);
        }

        return translatedResult;
    }

    private static Dictionary<LanguageType, SnapshotDictionary<string, string>> MergeLanguages(
        ILanguageResourcesConfig[] configs)
    {
        var merged = new Dictionary<LanguageType, Dictionary<string, string>>();
        foreach (var cfg in configs)
        {
            foreach (var (lang, dict) in cfg.Resources)
            {
                if (!merged.TryGetValue(lang, out var target))
                {
                    target = new Dictionary<string, string>();
                    merged[lang] = target;
                }

                foreach (var (k, v) in dict)
                {
                    target[k] = v;
                }
            }
        }

        var result = new Dictionary<LanguageType, SnapshotDictionary<string, string>>(merged.Count);
        foreach (var (lang, dict) in merged)
        {
            result[lang] = dict.ToSnapshotDictionary();
        }

        return result;
    }

    private void SaveTranslatedResult(LanguageType languageType, string ori, string translatedResult)
    {
        var cache = GetCurrentLangDictionary(languageType);
        cache.Add(ori, translatedResult);

        var resources = DefaultLanguageResourcesConfig.Resources;
        if (!resources.ContainsKey(languageType))
        {
            resources.Add(languageType, new Dictionary<string, string>());
        }


        if (resources[languageType].TryAdd(ori, translatedResult))
        {
            var currentProjectFolder = IOTools.GetCallerDirectoryPath();
            Debug.Assert(IOTools.DirectoryExists(currentProjectFolder));

            SettingsTools.SetToFolder(currentProjectFolder, DefaultLanguageResourcesConfig);
        }
    }

    private bool TryGetValueFromCache(LanguageType languageType, string text, out string result)
    {
        result = text;

        if (languageType == LanguageType.en)
        {
            return true;
        }

        var dic = GetCurrentLangDictionary(languageType);
        if (dic.TryGetValue(text, out var r))
        {
            result = r;
            return true;
        }

        return false;
    }

    private SnapshotDictionary<string, string> GetCurrentLangDictionary(LanguageType languageType)
    {
        if (!LanguageResources.ContainsKey(languageType))
        {
            LanguageResources.Add(languageType, new SnapshotDictionary<string, string>());
        }

        return LanguageResources[languageType];
    }
}
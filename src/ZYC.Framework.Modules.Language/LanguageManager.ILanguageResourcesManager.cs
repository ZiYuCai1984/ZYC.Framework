using ZYC.CoreToolkit.Common;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language;

internal partial class LanguageManager
{
    public LanguageResourceEntry UpdateLanguageResourceEntry(LanguageType languageType, string key, string value)
    {
        if (languageType == LanguageType.en)
        {
            throw new InvalidOperationException("English entries are source keys and cannot be edited.");
        }

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException("Resource key cannot be empty.", nameof(key));
        }

        var configs = GetAllLanguageResources();
        var config = ResolveEditableResourceConfig(configs, languageType, key);
        var resources = config.Resources;
        if (!resources.TryGetValue(languageType, out var languageResources))
        {
            languageResources = new Dictionary<string, string>();
            resources[languageType] = languageResources;
        }

        languageResources[key] = value;
        SetCachedLanguageResource(languageType, key, value);
        SaveLanguageResourcesConfig(config);

        //BannerManager.PromptRestart();

        return CreateLanguageResourceEntry(configs, languageType, key);
    }


    public LanguageResourceEntry[] GetLanguageResourceEntries(LanguageType languageType)
    {
        var configs = GetAllLanguageResources();
        var keys = GetAllResourceKeys(configs);

        return keys.Select(key => CreateLanguageResourceEntry(configs, languageType, key))
            .ToArray();
    }

    private static LanguageResourceEntry CreateLanguageResourceEntry(
        ILanguageResourcesConfig[] configs,
        LanguageType languageType,
        string key)
    {
        if (languageType == LanguageType.en)
        {
            return new LanguageResourceEntry(
                languageType,
                key,
                key,
                "Source key",
                true,
                false);
        }

        var value = string.Empty;
        var sourceName = nameof(DefaultLanguageResourcesConfig);
        var exists = false;

        foreach (var config in configs)
        {
            if (!config.Resources.TryGetValue(languageType, out var languageResources)
                || !languageResources.TryGetValue(key, out var candidate))
            {
                continue;
            }

            value = candidate;
            sourceName = ModuleNameTools.GetTypeModuleName(config.GetType());
            exists = true;
        }

        return new LanguageResourceEntry(
            languageType,
            key,
            value,
            sourceName,
            exists,
            true);
    }

    private static string[] GetAllResourceKeys(ILanguageResourcesConfig[] configs)
    {
        return configs.SelectMany(config => config.Resources.Values)
            .SelectMany(resources => resources.Keys)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(key => key, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
    }

    private ILanguageResourcesConfig ResolveEditableResourceConfig(
        ILanguageResourcesConfig[] configs,
        LanguageType languageType,
        string key)
    {
        ILanguageResourcesConfig? target = null;
        foreach (var config in configs)
        {
            if (!config.Resources.TryGetValue(languageType, out var languageResources)
                || !languageResources.ContainsKey(key))
            {
                continue;
            }

            target = config;
        }

        if (target is null || target is DefaultLanguageResourcesConfig)
        {
            return OverrideDefaultLanguageResourcesConfig;
        }

        return target;
    }

    private void SetCachedLanguageResource(LanguageType languageType, string key, string value)
    {
        var cache = GetCurrentLangDictionary(languageType);
        var resources = cache.ToDictionary(kv => kv.Key, kv => kv.Value);
        resources[key] = value;
        LanguageResources[languageType] = resources.ToSnapshotDictionary();
    }
}

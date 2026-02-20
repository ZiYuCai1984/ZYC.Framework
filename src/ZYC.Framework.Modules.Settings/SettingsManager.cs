using System.Diagnostics;
using System.Reflection;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.CoreToolkit.Extensions.Settings;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config.Attributes;
using ZYC.Framework.Modules.Settings.Abstractions;
using ZYC.Framework.Modules.Settings.Abstractions.Event;

namespace ZYC.Framework.Modules.Settings;

[RegisterSingleInstanceAs(typeof(ISettingsManager))]
public partial class SettingsManager : ISettingsManager
{
    public SettingsManager(
        IEventAggregator eventAggregator,
        IConfig[] configs,
        IAppContext appContext,
        ILifetimeScope lifetimeScope,
        IAppLogger<SettingsManager> logger)
    {
        EventAggregator = eventAggregator;
        Configs = configs;
        AppContext = appContext;
        LifetimeScope = lifetimeScope;
        Logger = logger;
    }

    private IEventAggregator EventAggregator { get; }

    private IConfig[] Configs { get; }

    private IAppContext AppContext { get; }

    private ILifetimeScope LifetimeScope { get; }

    private IAppLogger<SettingsManager> Logger { get; }

    private ISettingsView? SettingsView { get; set; }

    public void BringIntoView(Type configType)
    {
        SettingsView?.BringIntoView(configType);
    }

    public void BringIntoView<T>()
    {
        BringIntoView(typeof(T));
    }

    void ISettingsManager.SetSettingsView(ISettingsView? settingsView)
    {
        SettingsView = settingsView;
    }

    public SettingGroup[] GetSettingGroupsWithoutHidden()
    {
        var configs = new List<IConfig>();

        foreach (var config in Configs)
        {
            var type = config.GetType();
            if (type.ExistAttribute<HiddenAttribute>())
            {
                continue;
            }

            configs.Add(config);
        }

        return CreateSettingGroups(configs.ToArray());
    }

    public void SaveConfig<T>() where T : IConfig
    {
        var targetConfig = Configs.FirstOrDefault(t => t is T);
        if (targetConfig == null)
        {
            throw new InvalidOperationException($"Can not find config <{typeof(T)}> !!");
        }

        SettingsTools.SetToFolderGeneric(AppContext.GetMainAppDirectory(), targetConfig);
    }

    public Uri GetPageUri()
    {
        return SettingsModuleConstants.Uri;
    }

    public SettingGroup[] CreateSettingGroups<T>(T[] configs) where T : IConfig
    {
        var settingGroups = new List<SettingGroup>();
        foreach (var config in configs)
        {
            Debug.Assert(config != null);

            var type = config.GetType();

            var properties = type.GetProperties();
            var settingItems = new List<SettingItem>();

            foreach (var property in properties)
            {
                var isHidden = property.ExistAttribute<HiddenAttribute>();
                if (isHidden)
                {
                    continue;
                }


                var isMultiline = property.ExistAttribute<MultilineTextAttribute>();

                var initValue = property.GetValue(config);
                if (isMultiline)
                {
                    initValue = new MultilineText(initValue);
                }

                var settingItem = new SettingItem(
                    property.Name,
                    initValue,
                    (self, newValue) =>
                    {
                        if (isMultiline)
                        {
                            newValue = new MultilineText(newValue);
                            //!WARNING Design defeat
                            UpdateMultilineConfig(config, property, self, newValue, Logger);
                        }
                        else
                        {
                            UpdateConfig(config, property, self, newValue, Logger);
                        }
                    }, ResolveDescription(property));

                settingItems.Add(settingItem);
            }

            var settingGroup = new SettingGroup(
                type,
                settingItems.ToArray());

            settingGroups.Add(settingGroup);
        }

        return settingGroups.ToArray();
    }


    private static string? ResolveDescription(PropertyInfo property)
    {
        var attribute = property.GetCustomAttribute<DescriptionAttribute>();
        if (attribute == null)
        {
            return null;
        }

        return attribute.Value;
    }


    private void UpdateMultilineConfig(
        IConfig config,
        PropertyInfo property,
        SettingItem item,
        object? newValue,
        IAppLogger<SettingsManager> logger)
    {
        try
        {
            var oriConfig = JsonTools.DeepCopyGeneric(config);

            var parsedValue = (MultilineText)newValue!;

            var oldValue = property.GetValue(config)!;
            if (oldValue.Equals(newValue))
            {
                return;
            }

            property.SetValue(config, parsedValue.Text);
            item.Value = newValue;

            PublishSettingChangedEvent(
                config.GetType(), oriConfig, config);

            SettingsTools.SetToFolderGeneric(AppContext.GetMainAppDirectory(), config);
        }
        catch (Exception e)
        {
            logger.Error(e);
        }
    }

    private void UpdateConfig(
        IConfig config,
        PropertyInfo property,
        SettingItem item,
        object? newValue,
        IAppLogger<SettingsManager> logger)
    {
        try
        {
            var oriConfig = JsonTools.DeepCopyGeneric(config);

            var valueType = property.PropertyType;
            if (!TryParseValueFromString(valueType, newValue, out var parsedNewValue))
            {
                return;
            }

            newValue = parsedNewValue;

            var oldValue = property.GetValue(config)!;
            if (oldValue.Equals(newValue))
            {
                return;
            }

            property.SetValue(config, newValue);
            item.Value = newValue;

            PublishSettingChangedEvent(
                config.GetType(), oriConfig, config);

            SettingsTools.SetToFolderGeneric(AppContext.GetMainAppDirectory(), config);
        }
        catch (Exception e)
        {
            logger.Error(e);
        }
    }

    private void PublishSettingChangedEvent(Type type, object oldValue, object newValue)
    {
        EventAggregator.Publish(SettingChangedEvent.CreateSettingChangedEvent(
            type,
            oldValue,
            newValue));
    }

    private static bool TryParseValueFromString(
        Type valueType,
        object? value,
        out object? parsedValue)
    {
        parsedValue = null;


        if (valueType == typeof(string[]))
        {
            parsedValue = (string[])value!;
            return true;
        }

        var s = value?.ToString() ?? string.Empty;

        if (valueType == typeof(string))
        {
            parsedValue = s;
            return true;
        }

        if (valueType == typeof(bool))
        {
            if (bool.TryParse(s, out var result))
            {
                parsedValue = result;
                return true;
            }
        }

        if (valueType == typeof(int))
        {
            if (int.TryParse(s, out var result))
            {
                parsedValue = result;
                return true;
            }

            return false;
        }

        if (valueType == typeof(double))
        {
            if (double.TryParse(s, out var result))
            {
                parsedValue = result;
                return true;
            }

            return false;
        }


        if (Enum.TryParse(valueType, s, out var r))
        {
            parsedValue = r;
            return true;
        }

        return false;
    }
}
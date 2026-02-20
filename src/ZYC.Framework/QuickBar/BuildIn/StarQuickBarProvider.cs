using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.QuickBar;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.QuickBar.BuildIn;

[RegisterSingleInstanceAs(typeof(IStarQuickBarItemsProvider))]
internal class StarQuickBarProvider : IStarQuickBarItemsProvider
{
    private ISettingsManager? _settingsManager;

    public StarQuickBarProvider(
        IToastManager toastManager,
        IEventAggregator eventAggregator,
        ITabManager tabManager,
        ILifetimeScope lifetimeScope,
        StarQuickBarConfig starQuickBarConfig)
    {
        ToastManager = toastManager;
        EventAggregator = eventAggregator;
        TabManager = tabManager;

        LifetimeScope = lifetimeScope;
        StarQuickBarConfig = starQuickBarConfig;


        QuickMenuTitleItems = LoadFromConfig();
    }

    private ISettingsManager? SettingsManager
    {
        get
        {
            if (_settingsManager != null)
            {
                return _settingsManager;
            }

            if (LifetimeScope.TryResolve<ISettingsManager>(out var settingsManager))
            {
                _settingsManager = settingsManager;
            }

            return _settingsManager;
        }
    }

    private IToastManager ToastManager { get; }
    private IEventAggregator EventAggregator { get; }

    private ITabManager TabManager { get; }

    private ILifetimeScope LifetimeScope { get; }

    private StarQuickBarConfig StarQuickBarConfig { get; }

    private IQuickBarItem[] QuickMenuTitleItems { get; set; }

    public IQuickBarItem[] GetTitleMenuItems()
    {
        return QuickMenuTitleItems;
    }

    public void AttachItem<T>() where T : IQuickBarItem
    {
        AttachItem(LifetimeScope.Resolve<T>());
    }

    public void AttachItem(IQuickBarItem item)
    {
        var list = new List<IQuickBarItem>(QuickMenuTitleItems) { item };
        QuickMenuTitleItems = list.ToArray();
        InvokeQuickMenuItemsChanged();

        SaveToConfig();
    }

    public void DetachItem<T>() where T : IQuickBarItem
    {
        var array = QuickMenuTitleItems.Where(item => item.GetType() == typeof(T)).ToArray();

        foreach (var item in array)
        {
            DetachItem(item);
        }
    }

    public void DetachItem(IQuickBarItem item)
    {
        var list = new List<IQuickBarItem>(QuickMenuTitleItems);
        list.Remove(item);
        QuickMenuTitleItems = list.ToArray();
        InvokeQuickMenuItemsChanged();

        SaveToConfig();
    }


    public bool CheckIsStared(Uri uri)
    {
        foreach (var item in QuickMenuTitleItems)
        {
            if (item is not StarQuickBarItem s)
            {
                continue;
            }

            if (s.Uri == uri)
            {
                return true;
            }
        }

        return false;
    }

    public void DetachMenuItem(Uri uri)
    {
        var items = QuickMenuTitleItems.Where(t =>
        {
            if (t is not StarQuickBarItem item)
            {
                return false;
            }

            return item.Uri == uri;
        }).ToArray();

        foreach (var item in items)
        {
            DetachItem(item);
        }
    }

    public StarQuickBarItem CreateQuickMenuItem(Uri uri, string icon)
    {
        return new StarQuickBarItem(uri, icon, new RelayCommand(_ => true, _ =>
        {
            TabManager.NavigateAsync(uri);
        }));
    }

    private void InvokeQuickMenuItemsChanged()
    {
        EventAggregator.Publish(new QuickMenuItemsChangedEvent());
    }

    private void SaveToConfig()
    {
        var targets = new List<string>();
        foreach (var item in QuickMenuTitleItems)
        {
            if (item is StarQuickBarItem starItem)
            {
                targets.Add($"{starItem.Uri}|{starItem.Icon}");
            }
        }

        StarQuickBarConfig.Target = targets.ToArray();

        if (SettingsManager == null)
        {
            ToastManager.PromptMessage(ToastMessage.Warn("Save failed, missing <Settings> module !!"));
        }
        else
        {
            SettingsManager.SaveConfig<StarQuickBarConfig>();
        }
    }

    private IQuickBarItem[] LoadFromConfig()
    {
        var starQuickMenuItems = new List<IQuickBarItem>();

        foreach (var target in StarQuickBarConfig.Target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                continue;
            }

            var s = target.Split('|');
            if (s.Length != 2)
            {
                continue;
            }

            var uri = new Uri(s[0]);
            var icon = s[1];

            starQuickMenuItems.Add(CreateQuickMenuItem(uri, icon));
        }

        return starQuickMenuItems.ToArray();
    }
}
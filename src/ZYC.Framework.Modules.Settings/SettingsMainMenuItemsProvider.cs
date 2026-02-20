using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Settings;

[RegisterSingleInstanceAs(typeof(ISettingsMainMenuItemsProvider))]
internal class SettingsMainMenuItemsProvider : MainMenuItemsProvider, ISettingsMainMenuItemsProvider
{
    public SettingsMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Settings",
            Priority = MainMenuPriority.View + 10
        };

        RegisterSubItem<UserSettingsMainMenuItem>();
        RegisterSubItem<ResetAllMainMenuItem>();
    }

    public override MenuItemInfo Info { get; }
}
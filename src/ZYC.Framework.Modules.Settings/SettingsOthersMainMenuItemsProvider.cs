using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Settings;

[RegisterSingleInstanceAs(typeof(ISettingsOthersMainMenuItemsProvider))]
internal class SettingsOthersMainMenuItemsProvider : MainMenuItemsProvider, ISettingsOthersMainMenuItemsProvider
{
    public SettingsOthersMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Others",
            Icon = null,
            Anchor = SettingsMainMenuAnchors.Others,
            Priority = ToolsMainMenuPriority.Others
        };

        RegisterSubItem<ResetAllMainMenuItem>();
    }

    public override MenuItemInfo Info { get; }
}
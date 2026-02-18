using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Settings.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Settings;

[RegisterSingleInstance]
internal class UserSettingsMainMenuItem : MainMenuItem
{
    public UserSettingsMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Icon = SettingsModuleConstants.Icon,
            Title = SettingsModuleConstants.Title,
            Anchor = SettingMainMenuAnchors.Settings
        };

        Command = lifetimeScope.CreateNavigateCommand(SettingsModuleConstants.Uri);
    }
}
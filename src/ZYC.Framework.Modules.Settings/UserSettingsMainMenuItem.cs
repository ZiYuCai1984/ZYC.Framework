using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Settings;

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
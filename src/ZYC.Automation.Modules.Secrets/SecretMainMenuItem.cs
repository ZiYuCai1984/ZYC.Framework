using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Secrets.Abstractions;
using ZYC.Automation.Modules.Settings.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Secrets;

[RegisterSingleInstance]
internal class SecretMainMenuItem : MainMenuItem
{
    public SecretMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = SecretsModuleConstants.Title,
            Icon = SecretsModuleConstants.Icon,
            Anchor = SettingMainMenuAnchors.Settings
        };

        Command = lifetimeScope.CreateNavigateCommand(SecretsModuleConstants.Uri);
    }
}
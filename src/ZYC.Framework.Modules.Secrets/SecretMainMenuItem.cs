using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Secrets.Abstractions;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Secrets;

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
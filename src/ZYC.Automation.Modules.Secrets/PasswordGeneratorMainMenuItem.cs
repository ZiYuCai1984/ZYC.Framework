using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Secrets;

[RegisterSingleInstance]
internal class PasswordGeneratorMainMenuItem : MainMenuItem
{
    public PasswordGeneratorMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = PasswordGeneratorTabItem.Constants.Title,
            Icon = PasswordGeneratorTabItem.Constants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(PasswordGeneratorTabItem.Constants.Uri);
    }
}
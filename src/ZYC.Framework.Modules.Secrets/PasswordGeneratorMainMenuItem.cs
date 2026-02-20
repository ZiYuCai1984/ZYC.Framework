using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.Secrets;

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
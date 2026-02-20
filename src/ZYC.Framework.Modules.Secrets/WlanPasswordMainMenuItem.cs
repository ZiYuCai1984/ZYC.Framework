using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.Secrets;

[RegisterSingleInstance]
internal class WlanPasswordMainMenuItem : MainMenuItem
{
    public WlanPasswordMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = WlanPasswordTabItem.Constants.Title,
            Icon = WlanPasswordTabItem.Constants.Icon
        };

        Command = lifetimeScope.CreateNavigateCommand(WlanPasswordTabItem.Constants.Uri);
    }
}
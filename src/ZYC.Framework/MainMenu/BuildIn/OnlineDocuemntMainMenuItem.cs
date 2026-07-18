using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class OnlineDocuemntMainMenuItem : MainMenuItem
{
    public OnlineDocuemntMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Online Docuemnt",
            Icon = "Github",
            Anchor = AboutMainMenuAnchors.About
        };

        Command = lifetimeScope.CreateNavigateCommand(new Uri(ProductInfoExtended.DocumentUrl));
    }
}
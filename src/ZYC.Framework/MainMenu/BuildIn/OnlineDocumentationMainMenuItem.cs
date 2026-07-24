using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class OnlineDocumentationMainMenuItem : MainMenuItem
{
    public OnlineDocumentationMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Online Documentation",
            Icon = "Github",
            Anchor = AboutMainMenuAnchors.About
        };

        Command = lifetimeScope.CreateNavigateCommand(new Uri(ProductInfoExtended.DocumentUrl));
    }
}

using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IAboutMainMenuItemsProvider))]
internal class AboutMainMenuItemsProvider : MainMenuItemsProvider, IAboutMainMenuItemsProvider
{
    public AboutMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "About",
            Icon = null,
            Priority = MainMenuPriority.About
        };
    }

    public override MenuItemInfo Info { get; }
}
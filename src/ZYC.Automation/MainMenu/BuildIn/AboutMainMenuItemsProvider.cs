using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

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
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IViewMainMenuItemsProvider))]
internal class ViewMainMenuItemsProvider : MainMenuItemsProvider, IViewMainMenuItemsProvider
{
    public ViewMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "View",
            Icon = null,
            Priority = MainMenuPriority.View
        };
    }

    public override MenuItemInfo Info { get; }
}
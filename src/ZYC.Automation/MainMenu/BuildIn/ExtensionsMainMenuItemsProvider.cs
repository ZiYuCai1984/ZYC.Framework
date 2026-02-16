using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IExtensionsMainMenuItemsProvider))]
internal class ExtensionsMainMenuItemsProvider : MainMenuItemsProvider, IExtensionsMainMenuItemsProvider
{
    public ExtensionsMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Extensions",
            Icon = null,
            Priority = MainMenuPriority.Extensions
        };
    }

    public override MenuItemInfo Info { get; }
}
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

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
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IToolsMainMenuItemsProvider))]
internal class ToolsMainMenuItemsProvider : MainMenuItemsProvider, IToolsMainMenuItemsProvider
{
    public ToolsMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Tools",
            Icon = null,
            Priority = MainMenuPriority.Tools
        };
    }

    public override MenuItemInfo Info { get; }
}
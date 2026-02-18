using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

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
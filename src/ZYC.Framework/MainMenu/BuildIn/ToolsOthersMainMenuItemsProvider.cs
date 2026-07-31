using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IToolsOthersMainMenuItemsProvider))]
internal class ToolsOthersMainMenuItemsProvider : MainMenuItemsProvider, IToolsOthersMainMenuItemsProvider
{
    public ToolsOthersMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Others",
            Icon = null,
            Priority = ToolsMainMenuPriority.Others
        };

        RegisterSubItem<OverrideMutexIdMainMenuItem>();
    }

    public override MenuItemInfo Info { get; }
}
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Menu;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IFileMainMenuItemsProvider))]
internal class FileMainMenuItemsProvider : MainMenuItemsProvider, IFileMainMenuItemsProvider
{
    public FileMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "File",
            Icon = null,
            Priority = MainMenuPriority.File
        };
    }

    public override MenuItemInfo Info { get; }
}
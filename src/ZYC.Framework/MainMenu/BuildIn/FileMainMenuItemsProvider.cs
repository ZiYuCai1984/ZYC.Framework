using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

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
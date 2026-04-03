using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IFileNewMainMenuItemsProvider))]
internal class FileNewMainMenuItemsProvider : MainMenuItemsProvider, IFileNewMainMenuItemsProvider
{
    public FileNewMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "New"
        };
    }

    public override MenuItemInfo Info { get; }
}
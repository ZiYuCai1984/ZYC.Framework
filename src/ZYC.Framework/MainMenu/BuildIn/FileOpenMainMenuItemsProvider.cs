using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Menu;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstanceAs(typeof(IFileOpenMainMenuItemsProvider))]
internal class FileOpenMainMenuItemsProvider : MainMenuItemsProvider, IFileOpenMainMenuItemsProvider
{
    public FileOpenMainMenuItemsProvider(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = "Open"
        };
    }

    public override MenuItemInfo Info { get; }
}
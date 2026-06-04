using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.WebBrowser.Abstractions;
using ZYC.Framework.Modules.WebBrowser.ChromeWebStore;

namespace ZYC.Framework.Modules.WebBrowser;

internal class Module : ModuleBase
{
    public override string Icon => WebBrowserModuleConstants.MenuIcon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<WebBrowserTabItemFactory>();
        lifetimeScope.RegisterTabItemFactory<ChromeWebStoreExtensionManagerTabItemFactory>();

        var toolsMainMenuItemsProvider = lifetimeScope.Resolve<IToolsMainMenuItemsProvider>();
        toolsMainMenuItemsProvider.RegisterSubItem<WebBrowserMainMenuItem>();
        toolsMainMenuItemsProvider.RegisterSubItem<ChromeWebStoreExtensionManagerMainMenuItem>();

        return Task.CompletedTask;
    }
}

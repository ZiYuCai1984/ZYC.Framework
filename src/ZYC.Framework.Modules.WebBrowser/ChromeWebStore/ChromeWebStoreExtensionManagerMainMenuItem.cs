using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore;

[RegisterSingleInstance]
internal class ChromeWebStoreExtensionManagerMainMenuItem : MainMenuItem
{
    public ChromeWebStoreExtensionManagerMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Command = lifetimeScope.CreateNavigateCommand(WebBrowserModuleConstants.ChromeWebStoreExtensionManagerUri);
        Info = new MenuItemInfo
        {
            Title = WebBrowserModuleConstants.ChromeWebStoreExtensionManagerTitle,
            Icon = WebBrowserModuleConstants.ChromeWebStoreExtensionManagerIcon
        };
    }
}

using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser;

[RegisterSingleInstance]
public class WebBrowserMainMenuItem : MainMenuItem
{
    public WebBrowserMainMenuItem(
        ILifetimeScope lifetimeScope,
        WebBrowserConfig webBrowserConfig)
    {
        Command = lifetimeScope.CreateNavigateCommand(new Uri(webBrowserConfig.StartupUri));
        Info = new MenuItemInfo
        {
            Title = WebBrowserModuleConstants.MenuTitle,
            Icon = WebBrowserModuleConstants.MenuIcon
        };
    }
}
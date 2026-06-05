using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser;

internal class Module : ModuleBase
{
    public override string Icon => WebBrowserModuleConstants.MenuIcon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<WebBrowserTabItemFactory>();

        return Task.CompletedTask;
    }
}

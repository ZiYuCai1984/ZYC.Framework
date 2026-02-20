using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire;

internal class Module : ModuleBase
{
    public override string Icon => Base64IconResources.Aspire;

    public override async Task RegisterAsync(ContainerBuilder builder)
    {
        await Task.CompletedTask;
    }

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<AspireTabItemFactory>();
        lifetimeScope.RegisterToolsMainMenuItem<IAspireMainMenuItemsProvider>();

        lifetimeScope.RegisterDefaultStatucBarItem<AspireStatusBarItem>();

        var aspireBinarySourceConfig = lifetimeScope.Resolve<AspireConfig>();
        if (aspireBinarySourceConfig.AutoStart)
        {
            _ = Task.Run(async () =>
            {
                var aspireManager = lifetimeScope.Resolve<IAspireServiceManager>();
                await aspireManager.StartServerAsync();
            });
        }

        return Task.CompletedTask;
    }
}
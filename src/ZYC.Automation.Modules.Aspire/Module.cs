using Autofac;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Aspire.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.Aspire;

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
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server;

internal class Module : ModuleBase
{
    public override string Icon => Base64IconResources.MCP;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterToolsMainMenuItem<IMCPServerMainMenuItemsProvider>();


        var config = lifetimeScope.Resolve<MCPServerConfig>();
        if (config.AutoStart)
        {
            IDisposable mainWindowLoadedEvent = null!;

            // ReSharper disable once AsyncVoidLambda
            mainWindowLoadedEvent = lifetimeScope.SubscribeEvent<MainWindowLoadedEvent>(async _ =>
            {
                await lifetimeScope.Resolve<IMCPServerManager>().StartServerAsync();

                // ReSharper disable once AccessToModifiedClosure
                mainWindowLoadedEvent.Dispose();
            });
        }


        return Task.CompletedTask;
    }
}
using Autofac;
using ZYC.Automation.Abstractions;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.MCP.Server;

internal class Module : ModuleBase
{
    public override string Icon => Base64IconResources.MCP;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterToolsMainMenuItem<IMCPServerMainMenuItemsProvider>();
        lifetimeScope.RegisterDefaultStatucBarItem<MCPServerStatusBarItem>();

        return Task.CompletedTask;
    }
}
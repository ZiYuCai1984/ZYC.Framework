using Autofac;
using ZYC.Automation.Core;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.MCP.Server;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterToolsMainMenuItem<MCPServerMainMenuItem>();
        lifetimeScope.RegisterDefaultStatucBarItem<MCPServerStatusBarItem>();

        return Task.CompletedTask;
    }
}
using Autofac;
using ZYC.Automation.Core.Commands;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server.Commands;

[RegisterSingleInstance]
internal class StartServerCommand : AsyncPairCommandBase<StartServerCommand, StopServerCommand>
{
    public StartServerCommand(ILifetimeScope lifetimeScope, IMCPServerManager mcpServerManager) : base(lifetimeScope)
    {
        MCPServerManager = mcpServerManager;
    }

    private IMCPServerManager MCPServerManager { get; }

    public override bool CanExecute(object? parameter)
    {
        return MCPServerManager.GetStatusSnapshot().ServiceStatus == ServiceStatus.Stopped;
    }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        return MCPServerManager.StartServerAsync();
    }
}
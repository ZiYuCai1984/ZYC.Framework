using Autofac;
using ZYC.Automation.Core.Commands;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server.Commands;

[RegisterSingleInstance]
internal class StopServerCommand : AsyncPairCommandBase<StartServerCommand, StopServerCommand>
{
    public StopServerCommand(ILifetimeScope lifetimeScope, IMCPServerManager mcpServerManager) : base(lifetimeScope)
    {
        MCPServerManager = mcpServerManager;
    }

    private IMCPServerManager MCPServerManager { get; }

    public override bool CanExecute(object? parameter)
    {
        return MCPServerManager.GetStatusSnapshot().ServiceStatus == ServiceStatus.Running;
    }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        return MCPServerManager.StopServerAsync();
    }
}
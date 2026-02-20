using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server.Commands;

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
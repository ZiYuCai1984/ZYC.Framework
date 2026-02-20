using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server.Commands;

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
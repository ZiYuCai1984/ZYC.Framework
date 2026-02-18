using Autofac;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit;

namespace ZYC.Automation.Modules.MCP.Server.UI;

internal partial class MCPServerStatusBarItemView : IMCPServerManager
{
    private MCPServer? MCPServer { get; set; }

    private MCPServiceStatus MCPServiceStatus { get; set; } = MCPServiceStatus.Stopped();

    public Task StartServerAsync()
    {
        //TODO-zyc StartServerAsync(Temp code)
        try
        {
            MCPServer = LifetimeScope.Resolve<MCPServer>();
            _ = MCPServer.StartAsync(Dispatcher, MCPServerConfig.Port);

            MCPServiceStatus = MCPServiceStatus.Running();
        }
        catch (Exception e)
        {
            //ignore
        }

        return Task.CompletedTask;
    }

    public async Task StopServerAsync()
    {
        if (MCPServer == null)
        {
            DebuggerTools.Break();
            return;
        }

        MCPServiceStatus = MCPServiceStatus.Stopping();

        Exception? exception = null;

        try
        {
            await MCPServer.StopAsync();
        }
        catch (Exception e)
        {
            exception = e;
        }

        MCPServiceStatus = MCPServiceStatus.Stopped(exception);
    }

    public MCPServiceStatus GetStatusSnapshot()
    {
        return MCPServiceStatus;
    }
}
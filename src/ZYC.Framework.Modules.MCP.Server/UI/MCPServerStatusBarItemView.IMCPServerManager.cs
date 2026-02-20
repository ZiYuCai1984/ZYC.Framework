using Autofac;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server.UI;

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

            ToastManager.PromptMessage(new ToastMessage(
                $"Running at http://localhost:{MCPServerConfig.Port}",
                MCPServerModuleConstants.Icon, false));
        }
        catch (Exception _)
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
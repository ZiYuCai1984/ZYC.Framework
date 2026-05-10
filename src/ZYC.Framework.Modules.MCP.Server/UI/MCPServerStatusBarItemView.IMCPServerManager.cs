using Autofac;
using Microsoft.Extensions.Logging;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.MCP.Server.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server.UI;

internal partial class MCPServerStatusBarItemView : IMCPServerManager
{
    private readonly SemaphoreSlim _gate = new(1, 1);

    private ILogger<MCPServerStatusBarItemView> Logger { get; }

    private MCPServer? MCPServer { get; set; }

    private MCPServiceStatus MCPServiceStatus { get; set; } = MCPServiceStatus.Stopped();

    public async Task StartServerAsync()
    {
        await _gate.WaitAsync();

        try
        {
            if (MCPServer is not null)
            {
                return;
            }

            MCPServer? server = null;

            try
            {
                MCPServiceStatus = MCPServiceStatus.Starting();

                server = LifetimeScope.Resolve<MCPServer>();

                await server.StartAsync(MCPServerConfig.Port);

                MCPServer = server;
                MCPServiceStatus = MCPServiceStatus.Running();

                ToastManager.PromptMessage(new ToastMessage(
                    $"Running at http://127.0.0.1:{MCPServerConfig.Port}",
                    MCPServerModuleConstants.Icon,
                    false));
            }
            catch (Exception e)
            {
                Logger.Error(e);

                MCPServiceStatus = MCPServiceStatus.Stopped(e);

                if (server is not null)
                {
                    try
                    {
                        await server.StopAsync();
                    }
                    catch (Exception stopException)
                    {
                        Logger.Error(stopException);
                    }
                }

                MCPServer = null;

                ToastManager.PromptException(e);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task StopServerAsync()
    {
        await _gate.WaitAsync();

        try
        {
            var server = MCPServer;

            if (server is null)
            {
                return;
            }

            MCPServiceStatus = MCPServiceStatus.Stopping();

            Exception? exception = null;

            try
            {
                await server.StopAsync();
            }
            catch (Exception e)
            {
                exception = e;

                Logger.Error(e);
                ToastManager.PromptException(e);
            }
            finally
            {
                MCPServer = null;
                MCPServiceStatus = MCPServiceStatus.Stopped(exception);
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public MCPServiceStatus GetStatusSnapshot()
    {
        return MCPServiceStatus;
    }
}
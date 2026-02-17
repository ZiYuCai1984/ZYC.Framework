using System.Windows;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server.UI;

[RegisterSingleInstance]
internal partial class MCPServerStatusBarItemView
{
    public MCPServerStatusBarItemView(MCPServerHost mcpServerHost, MCPServerConfig mcpServerConfig)
    {
        MCPServerHost = mcpServerHost;
        MCPServerConfig = mcpServerConfig;

        InitializeComponent();
    }

    private MCPServerHost MCPServerHost { get; }

    private MCPServerConfig MCPServerConfig { get; }

    private bool FirstRending { get; set; } = true;


    private async void OnMCPServerStatusBarItemViewLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!FirstRending)
            {
                return;
            }

            FirstRending = false;
            await MCPServerHost.StartAsync(Dispatcher, MCPServerConfig.Port);
        }
        catch
        {
            //ignore
        }
    }
}
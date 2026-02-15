using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server.UI;

[RegisterSingleInstance]
internal partial class MCPServerStatusBarItemView
{
    public MCPServerStatusBarItemView()
    {
        InitializeComponent();
    }
}
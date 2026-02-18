using Autofac;
using ZYC.Automation.Abstractions.StatusBar;
using ZYC.Automation.Modules.MCP.Server.UI;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server;

[RegisterSingleInstance]
internal class MCPServerStatusBarItem : IStatusBarItem
{
    public MCPServerStatusBarItem(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    public object View => LifetimeScope.Resolve<MCPServerStatusBarItemView>();

    public StatusBarSection Section => StatusBarSection.Right;
}
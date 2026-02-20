using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.StatusBar;
using ZYC.Framework.Modules.MCP.Server.UI;

namespace ZYC.Framework.Modules.MCP.Server;

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
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.MCP.Server.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server;

[RegisterSingleInstance]
internal class MCPServerMainMenuItem : MainMenuItem
{
    public MCPServerMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = MCPServerModuleConstants.Title,
            Icon = MCPServerModuleConstants.Icon,
        };

        Command = lifetimeScope.CreateNavigateCommand(MCPServerModuleConstants.Uri);
    }
}
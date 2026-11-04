using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.MCP.Server.Abstractions;
using ZYC.Framework.Modules.MCP.Server.Commands;

namespace ZYC.Framework.Modules.MCP.Server;

[RegisterSingleInstance]
internal class InspectorMainMenuItem : MainMenuItem
{
    public InspectorMainMenuItem(NavigateToInspectorCommand navigateToInspectorCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Inspector",
            Icon = MCPServerModuleConstants.Icon
        };

        Command = navigateToInspectorCommand;
    }
}
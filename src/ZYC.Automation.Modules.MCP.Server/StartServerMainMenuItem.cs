using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Modules.MCP.Server.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server;

[RegisterSingleInstance]
internal class StartServerMainMenuItem : MainMenuItem
{
    public StartServerMainMenuItem(StartServerCommand startServerCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Start server",
            Icon = "PlayCircleOutline"
        };

        Command = startServerCommand;
    }
}
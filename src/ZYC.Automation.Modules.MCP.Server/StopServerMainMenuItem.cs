using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Modules.MCP.Server.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MCP.Server;

[RegisterSingleInstance]
internal class StopServerMainMenuItem : MainMenuItem
{
    public StopServerMainMenuItem(StopServerCommand stopServerCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Stop server",
            Icon = "PauseCircleOutline"
        };

        Command = stopServerCommand;
    }
}
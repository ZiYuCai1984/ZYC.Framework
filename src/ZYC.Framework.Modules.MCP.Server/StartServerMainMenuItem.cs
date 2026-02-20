using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.MCP.Server.Commands;

namespace ZYC.Framework.Modules.MCP.Server;

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
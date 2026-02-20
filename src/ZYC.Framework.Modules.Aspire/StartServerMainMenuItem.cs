using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.Aspire.Commands;

namespace ZYC.Framework.Modules.Aspire;

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
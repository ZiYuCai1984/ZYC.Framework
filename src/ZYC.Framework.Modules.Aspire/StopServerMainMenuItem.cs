using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.Aspire.Commands;

namespace ZYC.Framework.Modules.Aspire;

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
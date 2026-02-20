using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class RestartMainMenuItem : MainMenuItem
{
    public RestartMainMenuItem(RestartCommand restartCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Restart",
            Icon = "Restart",
            Anchor = FileMainMenuAnchors.Exit
        };

        Command = restartCommand;
    }
}
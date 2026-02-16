using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

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
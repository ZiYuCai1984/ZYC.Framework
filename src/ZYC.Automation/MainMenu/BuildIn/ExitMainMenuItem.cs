using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class ExitMainMenuItem : MainMenuItem
{
    public ExitMainMenuItem(ExitProcessCommand exitProcessCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Exit",
            Icon = "ExitToApp",
            Anchor = FileMainMenuAnchors.Exit
        };

        Command = exitProcessCommand;
    }
}
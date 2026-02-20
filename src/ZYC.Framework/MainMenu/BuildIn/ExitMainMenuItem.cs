using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.MainMenu.BuildIn;

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
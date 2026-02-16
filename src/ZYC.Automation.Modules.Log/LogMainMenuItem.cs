using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Modules.Log.Abstractions;
using ZYC.Automation.Modules.Log.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Log;

[RegisterSingleInstance]
internal class LogMainMenuItem : MainMenuItem
{
    public LogMainMenuItem(ILifetimeScope lifetimeScope)
    {
        Info = new MenuItemInfo
        {
            Title = LogModuleConstants.Title,
            Icon = LogModuleConstants.Icon,
            Anchor = FileMainMenuAnchors.Open
        };

        Command = lifetimeScope.Resolve<OpenLogFolderCommand>();
    }
}
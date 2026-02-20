using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.Log.Abstractions;
using ZYC.Framework.Modules.Log.Commands;

namespace ZYC.Framework.Modules.Log;

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
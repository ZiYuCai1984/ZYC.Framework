using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Commands;

namespace ZYC.Framework.MainMenu.BuildIn;

[RegisterSingleInstance]
internal class OverrideMutexIdMainMenuItem : MainMenuItem
{
    public OverrideMutexIdMainMenuItem(ShowOverrideMutexIdDialogCommand showOverrideMutexIdDialogCommand)
    {
        Info = new MenuItemInfo
        {
            Title = "Override Mutex Id"
        };


        Command = showOverrideMutexIdDialogCommand;
    }
}
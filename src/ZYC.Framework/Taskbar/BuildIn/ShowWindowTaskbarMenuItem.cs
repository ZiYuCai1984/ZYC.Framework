using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Taskbar.BuildIn;

[RegisterSingleInstance]
internal class ShowWindowTaskbarMenuItem : ITaskbarMenuItem
{
    public ShowWindowTaskbarMenuItem(ShowWindowCommand showWindowCommand)
    {
        ShowWindowCommand = showWindowCommand;
        Info = new MenuItemInfo
        {
            Title = "Show window"
        };
    }

    private ShowWindowCommand ShowWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => ShowWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}
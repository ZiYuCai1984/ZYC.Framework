using System.Windows.Input;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Abstractions.TaskbarMenu;
using ZYC.Automation.Core.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Taskbar.BuildIn;

[RegisterSingleInstance]
internal class HideWindowTaskbarMenuItem : ITaskbarMenuItem
{
    public HideWindowTaskbarMenuItem(HideWindowCommand showWindowCommand)
    {
        HideWindowCommand = showWindowCommand;
        Info = new MenuItemInfo
        {
            Title = "Hide window"
        };
    }

    private HideWindowCommand HideWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => HideWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}
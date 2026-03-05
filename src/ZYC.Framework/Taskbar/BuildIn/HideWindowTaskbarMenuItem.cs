using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Taskbar.BuildIn;

[RegisterSingleInstance]
internal class HideWindowTaskbarMenuItem : ITaskbarMenuItem
{
    public HideWindowTaskbarMenuItem(HideWindowCommand showWindowCommand)
    {
        HideWindowCommand = showWindowCommand;
        Info = new MenuItemInfo
        {
            Title = "Hide Window"
        };
    }

    private HideWindowCommand HideWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => HideWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}
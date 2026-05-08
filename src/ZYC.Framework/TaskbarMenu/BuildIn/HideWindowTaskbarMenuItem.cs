using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.TaskbarMenu.BuildIn;

[RegisterSingleInstance]
internal class HideWindowTaskbarMenuItem : ITaskbarMenuItem
{
    public HideWindowTaskbarMenuItem(HideWindowCommand showWindowCommand)
    {
        HideWindowCommand = showWindowCommand;
        Info = new MenuItemInfo
        {
            Title = "Hide Window",
            Priority = 40
        };
    }

    private HideWindowCommand HideWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => HideWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}

using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Taskbar.BuildIn;

[RegisterSingleInstance]
internal class ExitProcessTaskbarItem : ITaskbarMenuItem
{
    public ExitProcessTaskbarItem(ExitProcessCommand exitProcessCommand)
    {
        ExitProcessCommand = exitProcessCommand;

        Info = new MenuItemInfo
        {
            Title = "Exit"
        };
    }

    private ExitProcessCommand ExitProcessCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => ExitProcessCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}
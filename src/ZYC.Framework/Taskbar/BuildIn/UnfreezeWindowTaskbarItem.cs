using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Commands;

namespace ZYC.Framework.Taskbar.BuildIn;

[RegisterSingleInstance]
internal class UnfreezeWindowTaskbarItem : ITaskbarMenuItem
{
    public UnfreezeWindowTaskbarItem(UnfreezeWindowCommand unFreezeWindowCommand)
    {
        UnfreezeWindowCommand = unFreezeWindowCommand;

        Info = new MenuItemInfo
        {
            Title = "Unfreeze"
        };
    }

    private UnfreezeWindowCommand UnfreezeWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => UnfreezeWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}
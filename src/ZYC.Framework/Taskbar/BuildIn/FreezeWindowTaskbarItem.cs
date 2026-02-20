using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Commands;

namespace ZYC.Framework.Taskbar.BuildIn;

[RegisterSingleInstance]
internal class FreezeWindowTaskbarItem : ITaskbarMenuItem
{
    public FreezeWindowTaskbarItem(FreezeWindowCommand freezeWindowCommand)
    {
        FreezeWindowCommand = freezeWindowCommand;

        Info = new MenuItemInfo
        {
            Title = "Freeze"
        };
    }

    private FreezeWindowCommand FreezeWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => FreezeWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}
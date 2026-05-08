using System.Windows.Input;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.TaskbarMenu;
using ZYC.Framework.Commands;

namespace ZYC.Framework.TaskbarMenu.BuildIn;

[RegisterSingleInstance]
internal class FreezeWindowTaskbarItem : ITaskbarMenuItem
{
    public FreezeWindowTaskbarItem(FreezeWindowCommand freezeWindowCommand)
    {
        FreezeWindowCommand = freezeWindowCommand;

        Info = new MenuItemInfo
        {
            Title = "Freeze",
            Priority = 10
        };
    }

    private FreezeWindowCommand FreezeWindowCommand { get; }

    public MenuItemInfo Info { get; }

    public ICommand Command => FreezeWindowCommand;

    public ITaskbarMenuItem[] SubItems { get; } = [];
}

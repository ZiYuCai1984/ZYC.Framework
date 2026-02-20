using System.Windows.Input;
using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.Abstractions.TaskbarMenu;

public interface ITaskbarMenuItem
{
    MenuItemInfo Info { get; }

    ICommand Command { get; }

    ITaskbarMenuItem[] SubItems { get; }
}
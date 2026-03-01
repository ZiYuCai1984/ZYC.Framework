using System.Windows.Input;
using ZYC.Framework.Abstractions.MainMenu;

namespace ZYC.Framework.Abstractions.TaskbarMenu;

/// <summary>
///     Defines the contract for a menu item displayed in the taskbar or system tray.
///     Supports a hierarchical structure for nested sub-menus.
/// </summary>
public interface ITaskbarMenuItem
{
    /// <summary>
    ///     Gets the metadata and display information for the menu item,
    ///     such as text, icons, and visibility states.
    /// </summary>
    MenuItemInfo Info { get; }

    /// <summary>
    ///     Gets the command logic to be executed when the taskbar menu item is clicked.
    /// </summary>
    ICommand Command { get; }

    /// <summary>
    ///     Gets a collection of nested menu items, allowing for the creation of sub-menus.
    /// </summary>
    ITaskbarMenuItem[] SubItems { get; }
}
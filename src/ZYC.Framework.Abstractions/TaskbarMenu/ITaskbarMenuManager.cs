namespace ZYC.Framework.Abstractions.TaskbarMenu;

/// <summary>
///     Defines a manager responsible for handling the lifecycle and registration
///     of menu items within the application's taskbar or system tray context.
/// </summary>
public interface ITaskbarMenuManager
{
    /// <summary>
    ///     Registers a new menu item to be displayed in the taskbar menu.
    /// </summary>
    /// <param name="menuItem">The menu item instance to add.</param>
    void RegisterMenuItem(ITaskbarMenuItem menuItem);
}
namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     Represents a menu item that also acts as a container for sub-menu items.
///     This enables a recursive, hierarchical menu structure.
/// </summary>
public interface IMainMenuItemsProvider : IMainMenuItem
{
    /// <summary>
    ///     Registers a sub-menu item by its type under this provider.
    /// </summary>
    /// <typeparam name="T">The type of the sub-menu item to add.</typeparam>
    void RegisterSubItem<T>() where T : IMainMenuItem;

    /// <summary>
    ///     Registers an instance of a sub-menu item under this provider.
    /// </summary>
    /// <param name="mainMenuItem">The menu item instance to be added as a child.</param>
    void RegisterSubItem(IMainMenuItem mainMenuItem);
}
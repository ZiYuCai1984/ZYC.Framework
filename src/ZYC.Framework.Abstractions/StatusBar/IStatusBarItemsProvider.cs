namespace ZYC.Framework.Abstractions.StatusBar;

/// <summary>
///     Manages the registration and retrieval of status bar items,
///     allowing for modular contribution to the application's bottom bar.
/// </summary>
public interface IStatusBarItemsProvider
{
    /// <summary>
    ///     Retrieves all currently registered status bar items.
    /// </summary>
    /// <returns>An array of <see cref="IStatusBarItem" /> instances.</returns>
    IStatusBarItem[] GetStatusBarItems();

    /// <summary>
    ///     Registers a status bar item by its type.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IStatusBarItem" /> to register.</typeparam>
    void RegisterItem<T>() where T : IStatusBarItem;

    /// <summary>
    ///     Registers a specific instance of a status bar item.
    /// </summary>
    /// <param name="item">The status bar item instance to add.</param>
    void RegisterItem(IStatusBarItem item);

    /// <summary>
    ///     Unregisters a status bar item based on its type.
    /// </summary>
    /// <typeparam name="T">The type of the item to remove.</typeparam>
    void UnregisterItem<T>() where T : IStatusBarItem;

    /// <summary>
    ///     Unregisters a specific instance of a status bar item.
    /// </summary>
    /// <param name="item">The status bar item instance to remove.</param>
    void UnregisterItem(IStatusBarItem item);
}
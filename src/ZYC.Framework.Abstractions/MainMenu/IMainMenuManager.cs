namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     Defines the core manager responsible for the lifecycle and registration
///     of top-level menu items.
/// </summary>
public interface IMainMenuManager
{
    /// <summary>
    ///     Registers an existing instance of a menu item.
    /// </summary>
    /// <param name="item">The menu item instance to be added.</param>
    void RegisterItem(IMainMenuItem item);

    /// <summary>
    ///     Registers a menu item by its type.
    ///     Typically used with Dependency Injection to instantiate the item.
    /// </summary>
    /// <typeparam name="T">The type of the menu item to register.</typeparam>
    void RegisterItem<T>() where T : IMainMenuItem;

    /// <summary>
    ///     Gets all registered menu items in their raw (unsorted) collection state.
    /// </summary>
    /// <returns>An array of all registered <see cref="IMainMenuItem" /> instances.</returns>
    IMainMenuItem[] GetItems();

    /// <summary>
    ///     Gets all registered menu items, sorted based on their priority or predefined anchors.
    /// </summary>
    /// <returns>A sorted array of menu items. May contain null elements if the collection is sparse.</returns>
    IMainMenuItem?[] GetSortedItems();
}
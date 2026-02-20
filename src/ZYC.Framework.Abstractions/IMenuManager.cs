namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides registration and retrieval of menu items.
/// </summary>
/// <typeparam name="TItem">The menu item type.</typeparam>
public interface IMenuManager<TItem> where TItem : notnull
{
    /// <summary>
    ///     Registers a menu item instance.
    /// </summary>
    /// <param name="item">The menu item to register.</param>
    void RegisterItem(TItem item);

    /// <summary>
    ///     Registers a menu item type.
    /// </summary>
    /// <typeparam name="T">The menu item type to register.</typeparam>
    void RegisterItem<T>() where T : TItem;

    /// <summary>
    ///     Gets the registered menu items.
    /// </summary>
    /// <returns>The registered items.</returns>
    TItem[] GetItems();
}
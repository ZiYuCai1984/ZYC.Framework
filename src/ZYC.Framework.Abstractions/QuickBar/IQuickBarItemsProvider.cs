namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     Manages the collection of items displayed in the QuickBar.
///     Handles dynamic registration and removal of items.
/// </summary>
public interface IQuickBarItemsProvider
{
    /// <summary>
    ///     Retrieves all currently registered QuickBar items,
    ///     typically for rendering in the Title Bar or a specialized toolbar.
    /// </summary>
    /// <returns>An array of <see cref="IQuickBarItem" /> instances.</returns>
    IQuickBarItem[] GetTitleMenuItems();

    /// <summary>
    ///     Attaches a new item to the QuickBar by its type.
    ///     Often used with Dependency Injection for instantiation.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IQuickBarItem" /> to attach.</typeparam>
    void AttachItem<T>() where T : IQuickBarItem;

    /// <summary>
    ///     Attaches a specific instance of an item to the QuickBar.
    /// </summary>
    /// <param name="item">The item instance to add.</param>
    void AttachItem(IQuickBarItem item);

    /// <summary>
    ///     Removes an item of a specific type from the QuickBar.
    /// </summary>
    /// <typeparam name="T">The type of the item to detach.</typeparam>
    void DetachItem<T>() where T : IQuickBarItem;

    /// <summary>
    ///     Removes a specific item instance from the QuickBar.
    /// </summary>
    /// <param name="item">The item instance to remove.</param>
    void DetachItem(IQuickBarItem item);
}
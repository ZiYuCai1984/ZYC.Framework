namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     Orchestrates the registration and lifecycle of multiple <see cref="IQuickBarItemsProvider" />
///     instances to aggregate items into the main QuickBar UI.
/// </summary>
public interface IQuickBarManager
{
    /// <summary>
    ///     Registers a provider type that contributes items to the QuickBar.
    ///     Typically used with a Dependency Injection container to resolve the provider instance.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IQuickBarItemsProvider" /> to register.</typeparam>
    void RegisterQuickMenuItemsProvider<T>() where T : IQuickBarItemsProvider;

    /// <summary>
    ///     Unregisters a previously registered provider type,
    ///     removing all its contributed items from the QuickBar.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IQuickBarItemsProvider" /> to unregister.</typeparam>
    void UnregisterQuickMenuItemsProvider<T>() where T : IQuickBarItemsProvider;

    /// <summary>
    ///     Aggregates and returns a consolidated list of all items from all
    ///     registered providers to be displayed in the Title Bar menu.
    /// </summary>
    /// <returns>An array of <see cref="IQuickBarItem" /> instances collected from all providers.</returns>
    IQuickBarItem[] GetQuickMenuTitleItems();
}
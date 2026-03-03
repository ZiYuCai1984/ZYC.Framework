namespace ZYC.Framework.Abstractions.StatusBar;

/// <summary>
///     The central coordinator that manages all status bar contributions.
///     It aggregates items from various providers and categorizes them by their target sections.
/// </summary>
public interface IStatusBarManager
{
    /// <summary>
    ///     Registers a provider type that contributes items to the status bar.
    ///     Typically integrated with a Dependency Injection container.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IStatusBarItemsProvider" /> to register.</typeparam>
    void RegisterStatusBarItemsProvider<T>() where T : IStatusBarItemsProvider;

    /// <summary>
    ///     Unregisters a previously registered status bar provider type.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IStatusBarItemsProvider" /> to unregister.</typeparam>
    void UnregisterStatusBarItemsProvider<T>() where T : IStatusBarItemsProvider;

    /// <summary>
    ///     Retrieves a consolidated array of all items contributed by all registered providers.
    /// </summary>
    /// <returns>An array of all <see cref="IStatusBarItem" /> instances.</returns>
    IStatusBarItem[] GetAllItems();

    /// <summary>
    ///     Filters and retrieves all items assigned to the <see cref="StatusBarSection.Left" /> section.
    /// </summary>
    /// <returns>An array of status bar items to be displayed on the left side.</returns>
    IStatusBarItem[] GetLeftItems()
    {
        return GetAllItems().Where(t => t.Section == StatusBarSection.Left).ToArray();
    }

    /// <summary>
    ///     Filters and retrieves all items assigned to the <see cref="StatusBarSection.Right" /> section.
    /// </summary>
    /// <returns>An array of status bar items to be displayed on the right side.</returns>
    IStatusBarItem[] GetRightItems()
    {
        return GetAllItems().Where(t => t.Section == StatusBarSection.Right).ToArray();
    }
}
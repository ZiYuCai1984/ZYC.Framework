namespace ZYC.Automation.Abstractions.Tab;

/// <summary>
///     Manages the registration and lifecycle of tab item factories.
///     Acts as a central registry that the system queries to find the appropriate factory for a given URI.
/// </summary>
public interface ITabItemFactoryManager
{
    /// <summary>
    ///     Registers a new factory type into the manager.
    /// </summary>
    /// <typeparam name="T">The type of the factory to register. Must implement <see cref="ITabItemFactory" />.</typeparam>
    void RegisterFactory<T>() where T : ITabItemFactory;

    /// <summary>
    ///     Retrieves all registered tab item factories.
    ///     Typically, these are returned in an order determined by their <see cref="ITabItemFactory.Priority" />.
    /// </summary>
    /// <returns>An array of all available <see cref="ITabItemFactory" /> instances.</returns>
    ITabItemFactory[] GetTabItemFactories();
}
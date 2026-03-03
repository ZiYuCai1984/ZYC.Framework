namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Provides a base implementation for tab item factories.
///     Handles route discovery via attributes and basic URI matching logic.
/// </summary>
public abstract class TabItemFactoryBase : ITabItemFactory
{
    private TabItemRouteAttribute[]? _routes;

    /// <summary>
    ///     Gets the list of route attributes defined on the current factory class.
    ///     Results are cached after the first reflective access.
    /// </summary>
    protected IReadOnlyList<TabItemRouteAttribute> Routes
        => _routes ??= GetType().GetCustomAttributes(typeof(TabItemRouteAttribute), false)
            .Cast<TabItemRouteAttribute>()
            .ToArray();

    /// <summary>
    ///     Gets a value indicating whether this factory produces a singleton tab instance.
    ///     Default is true.
    /// </summary>
    public virtual bool IsSingle => true;

    /// <summary>
    ///     Gets the priority of this factory. Higher values may be processed first.
    /// </summary>
    public virtual int Priority => 0;

    /// <summary>
    ///     Asynchronously creates a new instance of a tab item.
    /// </summary>
    /// <param name="context">The context containing parameters for tab creation.</param>
    /// <returns>A task representing the asynchronous operation, containing the created tab instance.</returns>
    public abstract Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context);

    /// <summary>
    ///     Determines if this factory can handle the specified URI.
    /// </summary>
    /// <param name="uri">The navigation URI to check.</param>
    /// <returns>True if the factory supports the URI; otherwise, false.</returns>
    public virtual Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        return Task.FromResult(AttributeMightMatch(uri));
    }

    /// <summary>
    ///     Checks if any of the <see cref="TabItemRouteAttribute" /> defined on this class match the given URI.
    /// </summary>
    /// <param name="uri">The target URI.</param>
    /// <returns>True if at least one route attribute matches.</returns>
    protected bool AttributeMightMatch(Uri uri)
    {
        return Routes.Any(r => r.MightMatch(uri));
    }
}
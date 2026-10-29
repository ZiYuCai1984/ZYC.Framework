namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Defines a factory responsible for identifying and creating instances of tab items based on URIs.
/// </summary>
public interface ITabItemFactory
{
    /// <summary>
    ///     Gets whether this registered factory may have only one open tab across all workspaces.
    ///     If true, external navigation focuses its existing tab regardless of URI or query parameters,
    ///     preserving that tab's current URI. If false, external navigation creates a new tab.
    ///     This value does not control navigation within an existing tab.
    /// </summary>
    bool IsSingle { get; }

    /// <summary>
    ///     Gets the priority of this factory.
    ///     When multiple factories match the same URI, the one with the highest priority is selected.
    /// </summary>
    int Priority { get; }

    /// <summary>
    ///     Asynchronously creates a new <see cref="ITabItemInstance" /> using the provided creation context.
    /// </summary>
    /// <param name="context">Contextual information required to initialize the tab item.</param>
    /// <returns>A new instance of <see cref="ITabItemInstance" />.</returns>
    Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context);

    /// <summary>
    ///     Asynchronously determines whether this factory can handle the specified URI.
    /// </summary>
    /// <param name="uri">The URI to check.</param>
    /// <returns><c>true</c> if this factory can handle the URI; otherwise, <c>false</c>.</returns>
    Task<bool> CheckUriMatchedAsync(Uri uri);
}

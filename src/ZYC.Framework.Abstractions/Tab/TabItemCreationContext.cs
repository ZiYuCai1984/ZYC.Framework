namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Provides the necessary parameters and scope information required to create and initialize a new tab item instance.
/// </summary>
public class TabItemCreationContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TabItemCreationContext" /> class.
    /// </summary>
    /// <param name="uri">The target address for which the tab is being created.</param>
    /// <param name="lifetimeScope">
    ///     The dependency injection scope or container that governs the lifecycle of the tab's
    ///     dependencies.
    /// </param>
    public TabItemCreationContext(Uri uri, object lifetimeScope)
    {
        Uri = uri;
        LifetimeScope = lifetimeScope;
    }

    /// <summary>
    ///     Gets the URI that the new tab item should navigate to or represent.
    /// </summary>
    public Uri Uri { get; }

    /// <summary>
    ///     Gets the object representing the lifetime scope (often an Autofac <c>ILifetimeScope</c> or
    ///     a Microsoft <c>IServiceScope</c>) used to resolve services for the tab.
    /// </summary>
    public object LifetimeScope { get; }
}
namespace ZYC.Automation.Abstractions.Tab;

/// <summary>
///     Represents an active instance of a tab, managing its UI, metadata, and lifecycle.
/// </summary>
public interface ITabItemInstance : IDisposable
{
    /// <summary>
    ///     Gets the unique <see cref="TabReference" /> associated with this instance.
    /// </summary>
    TabReference TabReference { get; }

    /// <summary>
    ///     Gets the display title for the tab.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Gets the icon resource or key for the tab.
    /// </summary>
    string Icon { get; }

    /// <summary>
    ///     Gets the visual UI element (e.g., View or Control) for this tab.
    /// </summary>
    object View { get; }

    /// <summary>
    ///     Gets a value indicating whether the tab content supports localization.
    /// </summary>
    bool Localization { get; }

    /// <summary>
    ///     Asynchronously performs initialization or data loading for the tab.
    /// </summary>
    Task LoadAsync();

    /// <summary>
    ///     Invoked when the tab is about to close.
    ///     Returns <c>true</c> to allow closing, <c>false</c> to cancel.
    /// </summary>
    bool OnClosing();
}

/// <summary>
///     Provides extension members for <see cref="ITabItemInstance" /> to access underlying reference data directly.
/// </summary>
public static class ITabItemInstanceEx
{
    extension(ITabItemInstance instance)
    {
        /// <summary>
        ///     Gets the unique ID from the associated <see cref="TabReference" />.
        /// </summary>
        public Guid Id => instance.TabReference.Id;

        /// <summary>
        ///     Gets the current URI from the associated <see cref="TabReference" />.
        /// </summary>
        public Uri Uri => instance.TabReference.Uri;

        /// <summary>
        ///     Gets the scheme of the current URI.
        /// </summary>
        public string Scheme => instance.Uri.Scheme;

        /// <summary>
        ///     Gets the host of the current URI.
        /// </summary>
        public string Host => instance.Uri.Host;
    }
}
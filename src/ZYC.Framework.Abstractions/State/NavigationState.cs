namespace ZYC.Framework.Abstractions.State;

/// <summary>
///     Represents the persisted or current state of navigation within a specific workspace.
/// </summary>
public class NavigationState
{
    /// <summary>
    ///     Gets or sets the URI of the tab that currently has focus.
    ///     Returns null if no tab is focused.
    /// </summary>
    public Uri? Focus { get; set; }

    /// <summary>
    ///     Gets or sets the collection of URIs for all tabs currently open in the workspace.
    /// </summary>
    public Uri[] TabItems { get; set; } = [];

    /// <summary>
    ///     Gets or sets the sequential list of navigation history entries for the workspace.
    /// </summary>
    public HistoryItem[] History { get; set; } = [];

    /// <summary>
    ///     Represents a single entry in the navigation history.
    /// </summary>
    public class HistoryItem
    {
        /// <summary>
        ///     Gets or sets the date and time when this navigation occurred.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        ///     Gets or sets the destination address of the history entry.
        /// </summary>
        /// <remarks>
        ///     Note: This is stored as a <see cref="string" /> rather than a <see cref="Uri" />
        ///     to ensure compatibility with UI data binding mechanisms.
        /// </remarks>
        /// !WARNING In order to be compatible with the binding on the UI, type string is used instead of Uri.
        public string Uri { get; set; } = "";
    }
}
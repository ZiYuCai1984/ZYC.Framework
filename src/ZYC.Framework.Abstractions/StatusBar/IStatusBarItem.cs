namespace ZYC.Framework.Abstractions.StatusBar;

/// <summary>
///     Represents an individual visual element or status indicator to be displayed in the status bar.
/// </summary>
public interface IStatusBarItem
{
    /// <summary>
    ///     Gets the visual object (typically a UI component or ViewModel)
    ///     that represents this item in the interface.
    /// </summary>
    object View { get; }

    /// <summary>
    ///     Gets the specific region or slot within the status bar where this item should be anchored.
    /// </summary>
    StatusBarSection Section { get; }

    /// <summary>
    ///     Gets the sorting weight for this item within its assigned section.
    ///     Lower values usually position the item further to the start of the section.
    /// </summary>
    int Order => 0;
}
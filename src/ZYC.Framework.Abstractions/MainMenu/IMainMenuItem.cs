using System.Windows.Input;

namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     Defines a single item within the application's main navigation or menu system.
///     Supports hierarchical structures, icons, and display prioritization.
/// </summary>
public interface IMainMenuItem
{
    /// <summary>
    ///     Gets the command to execute when the menu item is selected or clicked.
    /// </summary>
    ICommand Command { get; }

    /// <summary>
    ///     Gets a collection of child menu items, allowing for nested or sub-menu structures.
    /// </summary>
    IMainMenuItem[] SubItems { get; }

    /// <summary>
    ///     Gets the display name or text of the menu item.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Gets the identifier, path, or key for the icon associated with this menu item.
    ///     Returns <c>null</c> if no icon is specified.
    /// </summary>
    string? Icon { get; }

    /// <summary>
    ///     Gets a unique anchor or identifier used for deep linking or UI positioning within the menu.
    /// </summary>
    string Anchor { get; }

    /// <summary>
    ///     Gets the sorting weight or priority of the item.
    ///     Higher values typically appear lower or further right in the list.
    /// </summary>
    int Priority { get; }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Title" /> should be localized
    ///     through a translation service.
    /// </summary>
    bool Localization { get; }

    /// <summary>
    ///     Gets a value indicating whether this menu item should be hidden from the UI,
    ///     regardless of its presence in the collection (e.g., for permission-based filtering).
    /// </summary>
    bool IsHidden { get; }

    /// <summary>
    ///     Gets the text string describing a keyboard shortcut or input gesture.
    ///     Example: "Ctrl+C" or "Alt+F4".
    /// </summary>
    string InputGestureText { get; }
}
using System.Windows.Input;

namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Defines the contract for a menu item within the workspace UI,
///     such as context menus or navigation bars.
/// </summary>
public interface IWorkspaceMenuItem
{
    /// <summary>
    ///     Gets the display text for the menu item.
    /// </summary>
    string Title { get; }

    /// <summary>
    ///     Gets the command logic to be executed when the menu item is clicked or invoked.
    /// </summary>
    ICommand? Command { get; }

    /// <summary>
    ///     Gets a collection of child menu items, allowing for nested workspace menu structures.
    /// </summary>
    IWorkspaceMenuItem[] SubItems { get; }

    /// <summary>
    ///     Gets the visual icon identifier for the menu item.
    ///     This typically refers to a path, a resource key, or a font icon glyph.
    /// </summary>
    string? Icon { get; }

    /// <summary>
    ///     Gets the anchor used to group or position this item within the workspace menu.
    /// </summary>
    string Anchor { get; }

    /// <summary>
    ///     Gets the visual ordering weight within the same anchor group.
    ///     Lower values appear first.
    /// </summary>
    int Priority { get; }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Title" /> represents a
    ///     localization key that needs to be translated before display.
    /// </summary>
    bool Localization { get; }
}

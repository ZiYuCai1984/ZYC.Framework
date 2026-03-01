using System.Windows.Input;

namespace ZYC.Framework.Abstractions.WindowTitle;

/// <summary>
///     Defines an interactive item to be displayed within the window title bar.
///     These items are typically rendered as small buttons or icons next to the window controls.
/// </summary>
public interface IWindowTitleItem
{
    /// <summary>
    ///     Gets the visual icon identifier for the title item.
    ///     This could be a resource key, a path, or a font icon glyph.
    /// </summary>
    string Icon { get; }

    /// <summary>
    ///     Gets the command logic to be executed when the title item is clicked or invoked.
    /// </summary>
    ICommand Command { get; }

    /// <summary>
    ///     Gets a value indicating whether the item should be currently visible in the title bar.
    /// </summary>
    bool IsVisible { get; }
}
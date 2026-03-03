using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     Represents an individual functional item within the QuickBar.
///     Each item binds a visual icon to an executable command.
/// </summary>
public interface IQuickBarItem
{
    /// <summary>
    ///     Gets the resource key, path, or identifier for the item's icon.
    /// </summary>
    string Icon { get; }

    /// <summary>
    ///     Gets the command to be executed when the user interacts with the item.
    /// </summary>
    ICommand Command { get; }

    /// <summary>
    ///     Gets the localized text to be displayed when the user hovers over the item.
    /// </summary>
    string Tooltip { get; }
}
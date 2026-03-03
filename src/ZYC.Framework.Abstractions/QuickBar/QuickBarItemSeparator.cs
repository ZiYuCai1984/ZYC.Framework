using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     Represents a visual separator within the QuickBar to group related items.
///     Typically rendered as a line or space, with no functional command.
/// </summary>
public class QuickBarItemSeparator : IQuickBarItem
{
    /// <summary> Returns an empty string as separators usually don't have icons. </summary>
    public string Icon { get; } = "";

    /// <summary> Returns null as separators are not interactable. </summary>
    public ICommand Command { get; } = null!;

    /// <summary> Returns an empty string for the tooltip. </summary>
    public string Tooltip { get; } = "";
}
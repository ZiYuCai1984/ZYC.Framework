using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     A standard implementation of a QuickBar item used for general functional buttons.
/// </summary>
public class SimpleQuickBarItem : QuickBarItemBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SimpleQuickBarItem" /> class.
    /// </summary>
    /// <param name="uri">The unique identifier or navigation target.</param>
    /// <param name="icon">The icon resource key or path.</param>
    /// <param name="command">The command to execute on click.</param>
    /// <param name="tooltip">The text displayed when hovering over the item.</param>
    public SimpleQuickBarItem(Uri uri, string icon, ICommand command, string tooltip = "")
        : base(uri, icon, command, tooltip)
    {
    }
}
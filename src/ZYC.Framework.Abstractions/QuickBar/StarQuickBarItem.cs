using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     Represents a "Starred" or "Favorite" item in the QuickBar.
///     This class can be used to distinguish bookmarked resources from standard functional items.
/// </summary>
public class StarQuickBarItem : QuickBarItemBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StarQuickBarItem" /> class.
    /// </summary>
    /// <param name="uri">The unique identifier of the starred resource.</param>
    /// <param name="icon">The icon representing the favorite item.</param>
    /// <param name="command">The command to open or interact with the starred resource.</param>
    /// <param name="tooltip">The localized description of the starred item.</param>
    public StarQuickBarItem(Uri uri, string icon, ICommand command, string tooltip = "")
        : base(uri, icon, command, tooltip)
    {
    }
}
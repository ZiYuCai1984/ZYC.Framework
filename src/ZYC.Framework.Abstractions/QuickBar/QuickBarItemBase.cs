using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

/// <summary>
///     Provides a base implementation for QuickBar items,
///     reducing boilerplate code for custom item types.
/// </summary>
public abstract class QuickBarItemBase : IQuickBarItem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="QuickBarItemBase" /> class.
    /// </summary>
    protected QuickBarItemBase(Uri uri, string icon, ICommand command, string tooltip = "")
    {
        Icon = icon;
        Command = command;
        Uri = uri;
        Tooltip = tooltip;
    }

    /// <summary> Gets the unique URI associated with this item. </summary>
    public Uri Uri { get; }

    /// <summary> Gets the command executed upon interaction. </summary>
    public ICommand Command { get; }

    /// <summary> Gets the icon identifier for the item. </summary>
    public string Icon { get; }

    /// <summary> Gets the localized tooltip text. </summary>
    public string Tooltip { get; }
}
namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     Represents the configuration and metadata for a single menu item in the navigation system.
/// </summary>
public class MenuItemInfo
{
    /// <summary>
    ///     Gets or sets the display text of the menu item.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the resource key or path for the menu item's icon.
    ///     Can be null if no icon is required.
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    ///     Gets or sets the unique identifier or navigation target (URL/Route)
    ///     that this menu item points to.
    /// </summary>
    public string Anchor { get; set; } = "";

    /// <summary>
    ///     Gets or sets the visual ordering weight.
    ///     Lower values typically appear first in the menu.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the <see cref="Title" />
    ///     should be processed through a localization provider.
    /// </summary>
    public bool Localization { get; set; } = true;
}
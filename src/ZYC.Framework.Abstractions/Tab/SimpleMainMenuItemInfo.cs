namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Represents the configuration information for a main menu item.
///     Contains display properties like title, icon, and localization settings.
/// </summary>
public class SimpleMainMenuItemInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SimpleMainMenuItemInfo" /> class.
    /// </summary>
    /// <param name="title">The display title or localization key of the menu item.</param>
    /// <param name="icon">The identifier for the menu item's icon.</param>
    /// <param name="localization">Determines if the title should be localized. Default is true.</param>
    public SimpleMainMenuItemInfo(
        string title,
        string icon,
        bool localization = true)
    {
        Title = title;
        Localization = localization;
        Icon = icon;
    }

    /// <summary>Gets the display title.</summary>
    public string Title { get; }

    /// <summary>Gets the icon identifier.</summary>
    public string Icon { get; }

    /// <summary>Gets a value indicating whether to enable localization for the title.</summary>
    public bool Localization { get; }
}
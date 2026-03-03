using System.Windows.Input;

namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     A concrete implementation of <see cref="IMainMenuItem" />.
///     Provides properties for menu metadata, sub-items, and command execution.
/// </summary>
public class MainMenuItem : IMainMenuItem
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MainMenuItem" /> class with default values.
    /// </summary>
    public MainMenuItem()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="MainMenuItem" /> class with specified properties.
    /// </summary>
    /// <param name="title">The display title of the menu item.</param>
    /// <param name="icon">The icon identifier or path.</param>
    /// <param name="command">The command to be executed when clicked.</param>
    /// <param name="anchor">The unique identifier for the menu's position or deep linking.</param>
    /// <param name="localization">Whether the title should be translated.</param>
    /// <param name="priority">The sorting weight (defaults to 0).</param>
    public MainMenuItem(
        string title,
        string? icon,
        ICommand command,
        string anchor = "",
        bool localization = true,
        int priority = 0)
    {
        Info = new MenuItemInfo
        {
            Title = title,
            Icon = icon,
            Anchor = anchor,
            Localization = localization,
            Priority = priority
        };

        Command = command;
    }

    /// <summary>
    ///     Gets or sets the metadata information object for this menu item.
    /// </summary>
    public MenuItemInfo Info { get; protected set; } = null!;

    /// <summary>
    ///     Gets or sets the command to execute when this item is selected.
    /// </summary>
    public ICommand Command { get; protected set; } = null!;

    /// <summary>
    ///     Gets or sets the array of child menu items. Defaults to an empty array.
    /// </summary>
    public IMainMenuItem[] SubItems { get; protected set; } = [];

    /// <summary>
    ///     Gets the title from the underlying <see cref="Info" /> object.
    /// </summary>
    public virtual string Title => Info.Title;

    /// <summary>
    ///     Gets the icon from the underlying <see cref="Info" /> object.
    /// </summary>
    public virtual string? Icon => Info.Icon;

    /// <summary>
    ///     Gets the anchor from the underlying <see cref="Info" /> object.
    /// </summary>
    public virtual string Anchor => Info.Anchor;

    /// <summary>
    ///     Gets the priority value from the underlying <see cref="Info" /> object.
    /// </summary>
    public virtual int Priority => Info.Priority;

    /// <summary>
    ///     Gets the localization flag from the underlying <see cref="Info" /> object.
    /// </summary>
    public virtual bool Localization => Info.Localization;

    /// <summary>
    ///     Gets or sets a value indicating whether the menu item is currently hidden.
    /// </summary>
    public virtual bool IsHidden { get; set; }
}
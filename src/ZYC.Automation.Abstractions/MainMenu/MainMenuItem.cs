using System.Windows.Input;

namespace ZYC.Automation.Abstractions.MainMenu;

public class MainMenuItem : IMainMenuItem
{
    public MainMenuItem()
    {
    }

    public MainMenuItem(
        string title,
        string icon,
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
    ///     !WARNING Info cannot be null(Design defeat!!)
    /// </summary>
    public MenuItemInfo Info { get; protected set; } = null!;

    public ICommand Command { get; protected set; } = null!;

    public IMainMenuItem[] SubItems { get; protected set; } = [];

    public virtual string Title => Info.Title;

    public virtual string? Icon => Info.Icon;

    public virtual string Anchor => Info.Anchor;

    public virtual int Priority => Info.Priority;

    public virtual bool Localization => Info.Localization;
}
namespace ZYC.Framework.Abstractions.Tab;

public class SimpleMainMenuItemInfo
{
    public SimpleMainMenuItemInfo(
        string title,
        string icon,
        bool localization = true)
    {
        Title = title;
        Localization = localization;
        Icon = icon;
    }

    public string Title { get; }

    public string Icon { get; }

    public bool Localization { get; }
}
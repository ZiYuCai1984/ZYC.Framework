namespace ZYC.Automation.Abstractions.MainMenu;

public class MenuItemInfo
{
    public string Title { get; set; } = string.Empty;

    public string? Icon { get; set; }

    public string Anchor { get; set; } = "";

    public int Priority { get; set; }

    public bool Localization { get; set; } = true;
}
using System.Windows.Input;

namespace ZYC.Framework.WebView2.Menu;

public class ExtendedMenuItem
{
    public string Title { get; set; } = "";

    public string Icon { get; set; } = "";

    public ICommand? Command { get; set; }

    public bool Localization { get; set; } = true;
}
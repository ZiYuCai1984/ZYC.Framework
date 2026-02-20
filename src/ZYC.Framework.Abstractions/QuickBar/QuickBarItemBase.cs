using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

public abstract class QuickBarItemBase : IQuickBarItem
{
    protected QuickBarItemBase(Uri uri, string icon, ICommand command, string tooltip = "")
    {
        Icon = icon;
        Command = command;
        Uri = uri;
        Tooltip = tooltip;
    }

    public Uri Uri { get; }

    public ICommand Command { get; }

    public string Icon { get; }

    public string Tooltip { get; }
}
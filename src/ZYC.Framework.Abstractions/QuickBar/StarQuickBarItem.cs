using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

public class StarQuickBarItem : QuickBarItemBase
{
    public StarQuickBarItem(Uri uri, string icon, ICommand command, string tooltip = "") : base(uri, icon, command,
        tooltip)
    {
    }
}
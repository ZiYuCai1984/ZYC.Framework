using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

public class SimpleQuickBarItem : QuickBarItemBase
{
    public SimpleQuickBarItem(Uri uri, string icon, ICommand command, string tooltip = "") : base(uri, icon, command,
        tooltip)
    {
    }
}
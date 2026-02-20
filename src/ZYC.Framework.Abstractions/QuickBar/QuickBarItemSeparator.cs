using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

public class QuickBarItemSeparator : IQuickBarItem
{
    public string Icon { get; } = "";

    public ICommand Command { get; } = null!;

    public string Tooltip { get; } = "";
}
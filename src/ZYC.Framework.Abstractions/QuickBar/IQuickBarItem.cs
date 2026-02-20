using System.Windows.Input;

namespace ZYC.Framework.Abstractions.QuickBar;

public interface IQuickBarItem
{
    string Icon { get; }

    ICommand Command { get; }

    string Tooltip { get; }
}
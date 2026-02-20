using System.Windows.Input;

namespace ZYC.Framework.Abstractions.WindowTitle;

public interface IWindowTitleItem
{
    string Icon { get; }

    ICommand Command { get; }

    bool IsVisible { get; }
}
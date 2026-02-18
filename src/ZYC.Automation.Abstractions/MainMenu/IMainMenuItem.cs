using System.Windows.Input;

namespace ZYC.Automation.Abstractions.MainMenu;

public interface IMainMenuItem
{
    public ICommand Command { get; }

    public IMainMenuItem[] SubItems { get; }

    public string Title { get; }

    public string? Icon { get; }

    public string Anchor { get; }

    public int Priority { get; }

    public bool Localization { get; }

    public bool IsHidden { get; }
}
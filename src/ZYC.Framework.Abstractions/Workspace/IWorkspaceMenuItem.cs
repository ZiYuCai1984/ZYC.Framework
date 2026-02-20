using System.Windows.Input;

namespace ZYC.Framework.Abstractions.Workspace;

public interface IWorkspaceMenuItem
{
    string Title { get; }

    ICommand Command { get; }

    string Icon { get; }

    bool Localization { get; }
}
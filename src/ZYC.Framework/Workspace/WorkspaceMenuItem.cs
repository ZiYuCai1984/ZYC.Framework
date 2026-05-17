using System.Windows.Input;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Workspace;

internal class WorkspaceMenuItem : IWorkspaceMenuItem
{
    public WorkspaceMenuItem(
        string title,
        ICommand command,
        string icon,
        bool localization = true,
        string anchor = "",
        int priority = 0,
        IWorkspaceMenuItem[]? subItems = null)
    {
        Title = title;
        Command = command;
        Icon = icon;
        Localization = localization;
        Anchor = anchor;
        Priority = priority;
        SubItems = subItems ?? [];
    }

    public string Title { get; }

    public ICommand Command { get; }

    public IWorkspaceMenuItem[] SubItems { get; }

    public string Icon { get; }

    public string Anchor { get; }

    public int Priority { get; }

    public bool Localization { get; }
}

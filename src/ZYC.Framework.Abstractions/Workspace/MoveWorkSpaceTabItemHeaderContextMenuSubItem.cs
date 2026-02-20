using System.Windows.Input;

namespace ZYC.Framework.Abstractions.Workspace;

public class MoveWorkSpaceTabItemHeaderContextMenuSubItem
{
    public MoveWorkSpaceTabItemHeaderContextMenuSubItem(WorkspaceNode workspace, string title, ICommand command)
    {
        Workspace = workspace;
        Title = title;
        Command = command;
    }

    public WorkspaceNode Workspace { get; }

    public string Title { get; }

    public ICommand Command { get; }

    public bool Localization => false;

    public MoveWorkSpaceTabItemHeaderContextMenuSubItem[] SubItems => [];

    public virtual string Icon => "";
}
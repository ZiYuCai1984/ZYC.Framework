using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Abstractions.Event;

public class WorkspaceHighlightEvent
{
    public WorkspaceHighlightEvent(WorkspaceNode workspaceNode, bool highlight)
    {
        WorkspaceNode = workspaceNode;
        Highlight = highlight;
    }

    public WorkspaceNode WorkspaceNode { get; }

    public bool Highlight { get; }
}
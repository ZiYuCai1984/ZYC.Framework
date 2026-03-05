using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised to toggle the visual highlight state of a specific workspace node,
///     often used during drag-and-drop or search result indexing.
/// </summary>
public sealed class WorkspaceHighlightEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WorkspaceHighlightEvent" /> class.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to be highlighted or unhighlighted.</param>
    /// <param name="highlight">A value indicating whether the highlight should be active.</param>
    public WorkspaceHighlightEvent(WorkspaceNode workspaceNode, bool highlight)
    {
        WorkspaceNode = workspaceNode;
        Highlight = highlight;
    }

    /// <summary>
    ///     Gets the workspace node associated with the highlight action.
    /// </summary>
    public WorkspaceNode WorkspaceNode { get; }

    /// <summary>
    ///     Gets a value indicating whether the highlight is enabled.
    /// </summary>
    public bool Highlight { get; }
}
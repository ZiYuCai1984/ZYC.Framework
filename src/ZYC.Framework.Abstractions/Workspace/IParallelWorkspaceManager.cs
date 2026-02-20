using ZYC.Framework.Abstractions.MCP;

namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Defines the contract for managing parallel workspaces, handling layout operations
///     such as splitting, merging, and workspace state management.
/// </summary>
[ExposeToMCP(InvokeOnUIThread = true)]
public interface IParallelWorkspaceManager
{
    /// <summary>
    ///     Sets the specified workspace node as the currently focused workspace.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to focus.</param>
    void SetFocusedWorkspace(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Retrieves the workspace node that currently has focus.
    /// </summary>
    /// <returns>The focused <see cref="WorkspaceNode" />.</returns>
    WorkspaceNode GetFocusedWorkspace();

    /// <summary>
    ///     Asynchronously splits the specified workspace node horizontally.
    /// </summary>
    /// <param name="workspaceNode">The target workspace node to split.</param>
    Task SplitHorizontalAsync(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Asynchronously splits the specified workspace node vertically.
    /// </summary>
    /// <param name="workspaceNode">The target workspace node to split.</param>
    Task SplitVerticalAsync(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Asynchronously merges the specified workspace node with its neighbor or parent container.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to be merged.</param>
    Task MergeAsync(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Asynchronously resets the workspace manager to its default state.
    /// </summary>
    Task ResetAsync();

    /// <summary>
    ///     Determines whether the specified workspace node can currently be merged.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to check.</param>
    /// <returns>True if merging is possible; otherwise, false.</returns>
    bool CanMerge(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Asynchronously toggles the layout orientation (horizontal/vertical) for the specified node.
    /// </summary>
    /// <param name="workspaceNode">The target workspace node.</param>
    Task ToggleOrientationAsync(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Determines whether the orientation of the specified workspace node can be toggled.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to check.</param>
    /// <returns>True if the orientation can be changed; otherwise, false.</returns>
    bool CanToggleOrientation(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Gets a read-only dictionary containing all workspace nodes indexed by their unique identifiers.
    /// </summary>
    /// <returns>A dictionary of <see cref="Guid" /> and <see cref="WorkspaceNode" /> pairs.</returns>
    IReadOnlyDictionary<Guid, WorkspaceNode> GetWorkspaceDictionary();

    /// <summary>
    ///     Asynchronously swaps the position of the specified workspace node with another relevant node.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to swap.</param>
    Task SwapAsync(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Determines whether the specified workspace node is eligible for a swap operation.
    /// </summary>
    /// <param name="workspaceNode">The workspace node to check.</param>
    /// <returns>True if swapping is allowed; otherwise, false.</returns>
    bool CanSwap(WorkspaceNode workspaceNode);

    /// <summary>
    ///     Updates the visual highlight state for a specific workspace node.
    /// </summary>
    /// <param name="workspace">The target workspace node.</param>
    /// <param name="highlight">True to enable highlighting, false to disable it.</param>
    void SetHighlight(WorkspaceNode workspace, bool highlight);

    /// <summary>
    ///     Asynchronously merges all existing workspace nodes into a single primary workspace.
    /// </summary>
    Task MergeAllAsync();
}
namespace ZYC.Automation.Abstractions.Workspace;

public interface IParallelWorkspaceManager
{
    void SetFocusedWorkspace(WorkspaceNode workspaceNode);

    WorkspaceNode GetFocusedWorkspace();

    Task SplitHorizontalAsync(WorkspaceNode workspaceNode);

    Task SplitVerticalAsync(WorkspaceNode workspaceNode);

    Task MergeAsync(WorkspaceNode workspaceNode);

    Task ResetAsync();

    bool CanMerge(WorkspaceNode workspaceNode);

    Task ToggleOrientationAsync(WorkspaceNode workspaceNode);

    bool CanToggleOrientation(WorkspaceNode workspaceNode);

    IReadOnlyDictionary<Guid, WorkspaceNode> GetWorkspaceDictionary();

    Task SwapAsync(WorkspaceNode workspaceNode);

    bool CanSwap(WorkspaceNode workspaceNode);

    void SetHighlight(WorkspaceNode workspace, bool highlight);

    Task MergeAllAsync();
}
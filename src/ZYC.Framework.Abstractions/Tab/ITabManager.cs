using ZYC.Framework.Abstractions.MCP;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Abstractions.Tab;

/// <summary>
///     Defines the core functionality for managing tab item instances across multiple workspaces,
///     handling navigation, lifecycle, and focus state.
/// </summary>
[ExposeToMCP(InvokeOnUIThread = true)]
public partial interface ITabManager
{
    /// <summary>
    ///     Gets the navigation state (e.g., history, current path) for a specific workspace.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace.</param>
    /// <returns>The navigation state object.</returns>
    NavigationState GetNavigationState(Guid workspaceId);

    /// <summary>
    ///     Retrieves the currently focused tab item instance in the specified workspace.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace.</param>
    /// <returns>The focused tab item instance, or null if no tab is focused.</returns>
    ITabItemInstance? GetFocusedTabItemInstance(Guid workspaceId);

    /// <summary>
    ///     Retrieves all tab item instances currently open in the specified workspace.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace.</param>
    /// <returns>An array of tab item instances.</returns>
    ITabItemInstance[] GetTabItemInstances(Guid workspaceId);


    /// <summary>
    ///     Asynchronously navigates to the specified URI in the current active context.
    /// </summary>
    /// <param name="uri">The target URI.</param>
    Task NavigateAsync(Uri uri);

    /// <summary>
    ///     Asynchronously navigates to the specified URI string in the current active context.
    /// </summary>
    /// <param name="uri">The target URI string.</param>
    Task NavigateAsync(string uri)
    {
        return NavigateAsync(new Uri(uri));
    }

    /// <summary>
    ///     Asynchronously navigates to a URI within a specific workspace.
    /// </summary>
    /// <param name="workspaceId">The target workspace identifier.</param>
    /// <param name="uri">The target URI.</param>
    Task NavigateAsync(Guid workspaceId, Uri uri);

    /// <summary>
    ///     Asynchronously navigates to a URI string within a specific workspace.
    /// </summary>
    /// <param name="workspaceId">The target workspace identifier.</param>
    /// <param name="uri">The target URI string.</param>
    Task NavigateAsync(Guid workspaceId, string uri)
    {
        return NavigateAsync(workspaceId, new Uri(uri));
    }

    /// <summary>
    ///     Transfers all tab item instances from a source workspace to a destination workspace.
    /// </summary>
    /// <param name="from">The source workspace ID.</param>
    /// <param name="to">The destination workspace ID.</param>
    void MoveAllTabItemInstances(Guid from, Guid to);

    /// <summary>
    ///     Searches for an existing tab with the specified URI and brings it into focus.
    /// </summary>
    /// <param name="uri">The URI of the tab to focus.</param>
    Task FocusAsync(Uri uri);

    /// <summary>
    ///     Reloads the content of the tab corresponding to the specified URI.
    /// </summary>
    /// <param name="uri">The URI of the tab to reload.</param>
    [Obsolete]
    Task ReloadAsync(Uri uri);

    /// <summary>
    ///     Asynchronously closes all open tabs across all workspaces.
    /// </summary>
    Task CloseAllAsync();
}

public partial interface ITabManager
{
    /// <summary>
    ///     Handles internal navigation within a tab, transitioning from an old URI to a new one.
    /// </summary>
    /// <param name="instance">The tab item instance to move.</param>
    /// <param name="oriUri">The original URI.</param>
    /// <param name="newUri">The new target URI.</param>
    [MCPIgnore]
    Task TabInternalNavigatingAsync(ITabItemInstance instance, Uri oriUri, Uri newUri);

    /// <summary>
    ///     Moves a tab item instance from one workspace to another.
    /// </summary>
    /// <param name="instance">The tab item instance to move.</param>
    /// <param name="from">The source workspace ID.</param>
    /// <param name="to">The destination workspace ID.</param>
    [MCPIgnore]
    void MoveTabItemInstance(ITabItemInstance instance, Guid from, Guid to);

    /// <summary>
    ///     Asynchronously closes a specific tab item instance.
    /// </summary>
    /// <param name="instance">The instance to be closed.</param>
    [MCPIgnore]
    Task CloseAsync(ITabItemInstance instance);

    /// <summary>
    ///     Asynchronously restores the manager's state (e.g., re-opening tabs from a previous session).
    /// </summary>
    [MCPIgnore]
    Task RestoreStateAsync();

    /// <summary>
    ///     Determines which workspace node a specific tab item instance belongs to.
    /// </summary>
    /// <param name="instance">The tab item instance to locate.</param>
    /// <returns>The workspace node containing the instance.</returns>
    [MCPIgnore]
    WorkspaceNode GetTabItemInstanceWorkspace(ITabItemInstance instance);

    /// <summary>
    ///     Sets a specific tab item instance as the focused tab within a workspace.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace.</param>
    /// <param name="instance">The tab item instance to focus.</param>
    [MCPIgnore]
    void SetFocusedTabItemInstance(Guid workspaceId, ITabItemInstance? instance);

    /// <summary>
    ///     Moves a specific tab item instance to a new position relative to a target tab item.
    /// </summary>
    /// <param name="source">The tab item instance that is being moved.</param>
    /// <param name="target">The reference tab item instance used to determine the new location.</param>
    /// <param name="position">The insertion mode (e.g., before or after) relative to the target tab.</param>
    [MCPIgnore]
    void MoveTabItemInstance(
        ITabItemInstance source,
        ITabItemInstance target,
        TabInsertPosition position);
}
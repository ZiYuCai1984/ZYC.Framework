using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when a tab is opened, focused, or reports a URI change in its owning workspace.
///     It does not imply that the tab gained focus or that its content finished loading.
/// </summary>
public sealed class NavigateCompletedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NavigateCompletedEvent" /> class.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace where navigation occurred.</param>
    /// <param name="tabItemInstance">The instance of the tab item that was navigated to.</param>
    public NavigateCompletedEvent(
        Guid workspaceId,
        ITabItemInstance tabItemInstance)
    {
        WorkspaceId = workspaceId;
        TabItemInstance = tabItemInstance;
    }

    /// <summary>
    ///     Gets the unique identifier of the workspace associated with this navigation.
    /// </summary>
    public Guid WorkspaceId { get; }

    /// <summary>
    ///     Gets the tab item instance associated with the navigation or URI change.
    /// </summary>
    public ITabItemInstance TabItemInstance { get; }
}

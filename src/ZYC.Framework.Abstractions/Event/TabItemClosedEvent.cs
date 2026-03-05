using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when a specific tab item has been closed within a workspace.
/// </summary>
public sealed class TabItemClosedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TabItemClosedEvent" /> class.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace where the tab was closed.</param>
    /// <param name="tabItemInstance">The instance of the tab item that was removed.</param>
    public TabItemClosedEvent(
        Guid workspaceId,
        ITabItemInstance tabItemInstance)
    {
        WorkspaceId = workspaceId;
        TabItemInstance = tabItemInstance;
    }

    /// <summary>
    ///     Gets the unique identifier of the workspace associated with this event.
    /// </summary>
    public Guid WorkspaceId { get; }

    /// <summary>
    ///     Gets the instance of the tab item that was closed.
    /// </summary>
    public ITabItemInstance TabItemInstance { get; }
}
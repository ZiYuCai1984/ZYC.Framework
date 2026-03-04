using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when the currently focused tab item in a workspace has changed.
/// </summary>
public sealed class FocusedTabItemChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="FocusedTabItemChangedEvent" /> class.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace where the focus changed.</param>
    /// <param name="tabItemInstance">The newly focused tab item instance, or <c>null</c> if no tab is focused.</param>
    public FocusedTabItemChangedEvent(
        Guid workspaceId,
        ITabItemInstance? tabItemInstance)
    {
        WorkspaceId = workspaceId;
        TabItemInstance = tabItemInstance;
    }

    /// <summary>
    ///     Gets the unique identifier of the workspace associated with this event.
    /// </summary>
    public Guid WorkspaceId { get; }

    /// <summary>
    ///     Gets the instance of the tab item that now has focus.
    ///     May be <c>null</c> if the focus was cleared.
    /// </summary>
    public ITabItemInstance? TabItemInstance { get; }
}
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when a navigation request within a workspace has successfully completed.
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
    ///     Gets the target tab item instance that is now active after navigation.
    /// </summary>
    public ITabItemInstance TabItemInstance { get; }
}
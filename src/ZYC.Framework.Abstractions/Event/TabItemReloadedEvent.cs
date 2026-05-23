using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when a tab item instance has been replaced by a freshly loaded instance.
/// </summary>
public sealed class TabItemReloadedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TabItemReloadedEvent" /> class.
    /// </summary>
    /// <param name="workspaceId">The unique identifier of the workspace where the tab was reloaded.</param>
    /// <param name="oldTabItemInstance">The tab item instance that was removed.</param>
    /// <param name="newTabItemInstance">The newly created tab item instance.</param>
    /// <param name="insertIndex">The index where the new tab item instance was inserted.</param>
    public TabItemReloadedEvent(
        Guid workspaceId,
        ITabItemInstance oldTabItemInstance,
        ITabItemInstance newTabItemInstance,
        int insertIndex)
    {
        WorkspaceId = workspaceId;
        OldTabItemInstance = oldTabItemInstance;
        NewTabItemInstance = newTabItemInstance;
        InsertIndex = insertIndex;
    }

    /// <summary>
    ///     Gets the unique identifier of the workspace associated with this reload.
    /// </summary>
    public Guid WorkspaceId { get; }

    /// <summary>
    ///     Gets the tab item instance that was removed.
    /// </summary>
    public ITabItemInstance OldTabItemInstance { get; }

    /// <summary>
    ///     Gets the newly created tab item instance.
    /// </summary>
    public ITabItemInstance NewTabItemInstance { get; }

    /// <summary>
    ///     Gets the index where the new tab item instance was inserted.
    /// </summary>
    public int InsertIndex { get; }
}

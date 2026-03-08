using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when one or more tab items are moved from one workspace to another.
/// </summary>
public sealed class TabItemsMovedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TabItemsMovedEvent" /> class.
    /// </summary>
    /// <param name="fromWorkspaceId">The unique identifier of the source workspace.</param>
    /// <param name="toWorkspaceId">The unique identifier of the destination workspace.</param>
    /// <param name="tabItems">An array of tab item instances being moved.</param>
    /// <param name="insertIndex">
    ///     The zero-based index where the items are inserted in the destination workspace, or null if
    ///     appended.
    /// </param>
    public TabItemsMovedEvent(
        Guid fromWorkspaceId,
        Guid toWorkspaceId,
        ITabItemInstance[] tabItems,
        int? insertIndex = null)
    {
        FromWorkspaceId = fromWorkspaceId;
        ToWorkspaceId = toWorkspaceId;
        TabItems = tabItems;
        InsertIndex = insertIndex;
    }

    /// <summary>
    ///     Gets the unique identifier of the workspace the items were moved from.
    /// </summary>
    public Guid FromWorkspaceId { get; }

    /// <summary>
    ///     Gets the unique identifier of the workspace the items were moved to.
    /// </summary>
    public Guid ToWorkspaceId { get; }

    /// <summary>
    ///     Gets the collection of tab item instances involved in the move.
    /// </summary>
    public ITabItemInstance[] TabItems { get; }

    /// <summary>
    ///     Gets the specific index in the destination workspace where the items were inserted.
    /// </summary>
    public int? InsertIndex { get; }
}
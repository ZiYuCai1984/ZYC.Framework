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
    public TabItemsMovedEvent(
        Guid fromWorkspaceId,
        Guid toWorkspaceId,
        ITabItemInstance[] tabItems)
    {
        FromWorkspaceId = fromWorkspaceId;
        ToWorkspaceId = toWorkspaceId;
        TabItems = tabItems;
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
}
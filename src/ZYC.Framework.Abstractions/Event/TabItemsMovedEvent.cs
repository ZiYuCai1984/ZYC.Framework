using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

public class TabItemsMovedEvent
{
    public TabItemsMovedEvent(
        Guid fromWorkspaceId,
        Guid toWorkspaceId,
        ITabItemInstance[] tabItems)
    {
        FromWorkspaceId = fromWorkspaceId;
        ToWorkspaceId = toWorkspaceId;
        TabItems = tabItems;
    }

    public Guid FromWorkspaceId { get; }

    public Guid ToWorkspaceId { get; }

    public ITabItemInstance[] TabItems { get; }
}
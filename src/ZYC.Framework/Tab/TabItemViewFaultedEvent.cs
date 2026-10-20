using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Tab;

internal class TabItemViewFaultedEvent
{
    public TabItemViewFaultedEvent(
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

    public Guid WorkspaceId { get; }
    public ITabItemInstance OldTabItemInstance { get; }
    public ITabItemInstance NewTabItemInstance { get; }
    public int InsertIndex { get; }
}

using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Tab;

internal partial class TabManager
{
    private void InvokeNavigateCompletedEvent(Guid workspaceId, ITabItemInstance instance)
    {
        EventAggregator.Publish(new NavigateCompletedEvent(workspaceId, instance));
    }

    private void InvokeFocusedTabItemChangedEvent(Guid workspaceId, ITabItemInstance? instance)
    {
        EventAggregator.Publish(new FocusedTabItemChangedEvent(workspaceId, instance));
    }


    private void InvokeCloseTabItemEvent(Guid workspaceId, ITabItemInstance instance)
    {
        EventAggregator.Publish(new TabItemClosedEvent(workspaceId, instance));
    }

    private void InvokeTabItemReloadedEvent(
        Guid workspaceId,
        ITabItemInstance oldInstance,
        ITabItemInstance newInstance,
        int insertIndex)
    {
        EventAggregator.Publish(new TabItemReloadedEvent(workspaceId, oldInstance, newInstance, insertIndex));
    }

    public void InvokeTabItemsRestoreCompletedEvent()
    {
        DebuggerTools.CheckCalledOnce();

        EventAggregator.Publish(new TabItemsRestoreCompletedEvent());
    }

    private void InvokeTabItemsMovedEvent(
        Guid fromWorkspaceId,
        Guid toWorkspaceId,
        ITabItemInstance[] instances, 
        int? insertIndex = null)
    {
        EventAggregator.Publish(new TabItemsMovedEvent(fromWorkspaceId, toWorkspaceId, instances, insertIndex));
    }
}

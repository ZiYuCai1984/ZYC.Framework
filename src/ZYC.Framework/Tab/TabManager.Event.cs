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

    public void InvokeTabItemsRestoreCompletedEvent()
    {
        DebuggerTools.CheckCalledOnce();

        EventAggregator.Publish(new TabItemsRestoreCompletedEvent());
    }

    private void InvokeTabItemsMovedEvent(
        Guid fromWorkspaceId,
        Guid toWorkspaceId,
        ITabItemInstance[] instances)
    {
        EventAggregator.Publish(new TabItemsMovedEvent(fromWorkspaceId, toWorkspaceId, instances));
    }
}
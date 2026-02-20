using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.Event;

public class NavigateCompletedEvent
{
    public NavigateCompletedEvent(
        Guid workspaceId,
        ITabItemInstance tabItemInstance)
    {
        WorkspaceId = workspaceId;
        TabItemInstance = tabItemInstance;
    }

    public Guid WorkspaceId { get; }

    public ITabItemInstance TabItemInstance { get; }
}
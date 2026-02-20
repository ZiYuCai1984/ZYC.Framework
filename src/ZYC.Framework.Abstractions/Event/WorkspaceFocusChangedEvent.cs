namespace ZYC.Framework.Abstractions.Event;

public class WorkspaceFocusChangedEvent
{
    public WorkspaceFocusChangedEvent(Guid? id = null)
    {
        Id = id;
    }

    public Guid? Id { get; }
}
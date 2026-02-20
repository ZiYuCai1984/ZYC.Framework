namespace ZYC.Framework.Modules.Update.Abstractions.Event;

public class UpdateContextChangedEvent
{
    public UpdateContextChangedEvent(UpdateContext updateContext)
    {
        UpdateContext = updateContext;
    }

    public UpdateContext UpdateContext { get; }
}
namespace ZYC.Framework.Modules.Update.Abstractions.Event;

/// <summary>
///     Represents an event raised when the update context has changed.
/// </summary>
public sealed class UpdateContextChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateContextChangedEvent" /> class.
    /// </summary>
    /// <param name="updateContext">The new update context.</param>
    public UpdateContextChangedEvent(UpdateContext updateContext)
    {
        UpdateContext = updateContext;
    }

    /// <summary>
    ///     Gets the current update context associated with the event.
    /// </summary>
    public UpdateContext UpdateContext { get; }
}
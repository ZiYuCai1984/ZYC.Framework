namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides event publishing and subscription capabilities.
/// </summary>
public interface IEventAggregator
{
    /// <summary>
    ///     Observes all published events.
    /// </summary>
    IObservable<object> ObserveAll();

    /// <summary>
    ///     Observes events of the specified type.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    IObservable<TEvent> Observe<TEvent>() where TEvent : notnull;

    /// <summary>
    ///     Subscribes to an event type.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="handler">The handler to invoke.</param>
    /// <param name="onUiThread">Whether to dispatch on the UI thread.</param>
    /// <returns>A disposable subscription token.</returns>
    IDisposable Subscribe<TEvent>(Action<TEvent> handler, bool onUiThread = false) where TEvent : notnull;

    /// <summary>
    ///     Publishes an event.
    /// </summary>
    /// <typeparam name="TEvent">The event type.</typeparam>
    /// <param name="eventData">The event instance.</param>
    void Publish<TEvent>(TEvent eventData) where TEvent : notnull;

    /// <summary>
    ///     Publishes an event as an object.
    /// </summary>
    /// <param name="eventData">The event instance.</param>
    void Publish(object eventData);
}
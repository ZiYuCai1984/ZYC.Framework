namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides a lightweight event aggregation mechanism.
/// </summary>
public interface IEventAggregator
{
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
    /// <param name="event">The event instance.</param>
    void Publish<TEvent>(TEvent @event) where TEvent : notnull;

    /// <summary>
    ///     Publishes an event as an object.
    /// </summary>
    /// <param name="obj">The event instance.</param>
    void Publish(object obj);
}
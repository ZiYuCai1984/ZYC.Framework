using System.Collections.Concurrent;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Infrastructure;

[RegisterSingleInstanceAs(typeof(IEventAggregator))]
public sealed class EventAggregator : IEventAggregator
{
    private readonly ConcurrentDictionary<Type, List<WeakReference>> _map = new();
    private readonly SynchronizationContext? _uiCtx;

    public EventAggregator(IAppContext appContext)
    {
        _uiCtx = appContext.GetUISynchronizationContext();
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler, bool onUiThread = false) where TEvent : notnull
    {
        var list = _map.GetOrAdd(typeof(TEvent), _ => []);
        var entry = new Subscription<TEvent>(handler, onUiThread, _uiCtx);
        lock (list)
        {
            list.Add(new WeakReference(entry));
        }

        return new Unsubscriber(() =>
        {
            lock (list)
            {
                list.RemoveAll(w => !w.IsAlive || ReferenceEquals(w.Target, entry));
            }
        });
    }

    public void Publish<TEvent>(TEvent @event) where TEvent : notnull
    {
        var eventType = @event.GetType();

        if (!_map.TryGetValue(eventType, out var list))
        {
            return;
        }

        List<Subscription<TEvent>> targets;
        lock (list)
        {
            targets = list.Select(w => w.Target as Subscription<TEvent>)
                .Where(x => x is not null)
                .Cast<Subscription<TEvent>>()
                .ToList();
            list.RemoveAll(w => !w.IsAlive);
        }

        foreach (var s in targets)
        {
            s.Invoke(@event);
        }
    }

    public void Publish(object @event)
    {
        if (@event is null)
        {
            return;
        }

        var eventType = @event.GetType();
        if (!_map.TryGetValue(eventType, out var list))
        {
            return;
        }

        List<object> targets;
        lock (list)
        {
            targets = list
                .Select(w => w.Target)
                .Where(t => t is not null)
                .ToList()!;
            list.RemoveAll(w => !w.IsAlive);
        }

        foreach (var target in targets)
        {
            ((dynamic)target).Invoke((dynamic)@event);
        }
    }

    private sealed class Subscription<TEvent>
    {
        private readonly SynchronizationContext? _ctx;
        private readonly Action<TEvent> _handler;
        private readonly bool _onUiThread;

        public Subscription(Action<TEvent> handler, bool onUiThread, SynchronizationContext? ctx)
        {
            _handler = handler;
            _onUiThread = onUiThread;
            _ctx = ctx;
        }

        public void Invoke(TEvent e)
        {
            if (_onUiThread && _ctx is not null)
            {
                _ctx.Post(_ => _handler(e), null);
            }
            else
            {
                _handler(e);
            }
        }
    }

    private sealed class Unsubscriber : IDisposable
    {
        private readonly Action _dispose;

        public Unsubscriber(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose();
        }
    }
}
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Infrastructure;

[RegisterSingleInstanceAs(typeof(IEventAggregator))]
public sealed class EventAggregator : IEventAggregator, IDisposable
{
    private readonly ISubject<object> _subject;
    private readonly SynchronizationContext? _uiCtx;
    private bool _disposed;

    public EventAggregator(IAppContext appContext)
    {
        _uiCtx = appContext.GetUISynchronizationContext();
        _subject = Subject.Synchronize(new Subject<object>());
    }

    public IObservable<object> ObserveAll()
    {
        ThrowIfDisposed();
        return _subject.AsObservable();
    }

    public IObservable<TEvent> Observe<TEvent>() where TEvent : notnull
    {
        ThrowIfDisposed();
        return _subject.OfType<TEvent>();
    }

    public IDisposable Subscribe<TEvent>(Action<TEvent> handler, bool onUiThread = false) where TEvent : notnull
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(handler);

        return Observe<TEvent>().Subscribe(e =>
        {
            if (onUiThread && _uiCtx is not null)
            {
                _uiCtx.Post(static state =>
                {
                    var s = (DispatchState<TEvent>)state!;
                    s.Handler(s.EventData);
                }, new DispatchState<TEvent>(handler, e));
            }
            else
            {
                handler(e);
            }
        });
    }

    public void Publish<TEvent>(TEvent eventData) where TEvent : notnull
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(eventData);

        _subject.OnNext(eventData);
    }

    public void Publish(object eventData)
    {
        ThrowIfDisposed();

        if (eventData is null)
        {
            return;
        }

        _subject.OnNext(eventData);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _subject.OnCompleted();

        if (_subject is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }

    private sealed class DispatchState<TEvent>
    {
        public DispatchState(Action<TEvent> handler, TEvent eventData)
        {
            Handler = handler;
            EventData = eventData;
        }

        public Action<TEvent> Handler { get; }

        public TEvent EventData { get; }
    }
}
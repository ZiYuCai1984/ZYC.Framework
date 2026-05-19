using Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework;

internal sealed class StartupUriNavigationDispatcher : IDisposable
{
    private readonly Queue<Uri> _pendingUris = new();

    private readonly object _syncRoot = new();

    private bool _canNavigate;

    private bool _isDraining;

    private ILifetimeScope? _lifetimeScope;

    private IDisposable? _restoreCompletedSubscription;

    public void Register(ILifetimeScope lifetimeScope, Uri? startupUri)
    {
        lock (_syncRoot)
        {
            _lifetimeScope = lifetimeScope;
        }

        _restoreCompletedSubscription = lifetimeScope.Resolve<IEventAggregator>()
            .Subscribe<TabManagerRestoreCompleted>(_ =>
            {
                lock (_syncRoot)
                {
                    _canNavigate = true;
                }

                DrainAsync();
            }, true);

        if (startupUri != null)
        {
            Enqueue(startupUri);
        }
    }

    public void Enqueue(Uri uri)
    {
        lock (_syncRoot)
        {
            _pendingUris.Enqueue(uri);
        }

        DrainAsync();
    }

    public void Dispose()
    {
        _restoreCompletedSubscription?.Dispose();
    }

    private void DrainAsync()
    {
        lock (_syncRoot)
        {
            if (!_canNavigate || _lifetimeScope == null || _isDraining)
            {
                return;
            }

            _isDraining = true;
        }

        _ = DrainPendingUrisAsync();
    }

    private async Task DrainPendingUrisAsync()
    {
        while (true)
        {
            Uri uri;
            ILifetimeScope lifetimeScope;

            lock (_syncRoot)
            {
                if (!_canNavigate || _lifetimeScope == null || _pendingUris.Count == 0)
                {
                    _isDraining = false;
                    return;
                }

                uri = _pendingUris.Dequeue();
                lifetimeScope = _lifetimeScope;
            }

            await NavigateAsync(lifetimeScope, uri);
        }
    }

    private static async Task NavigateAsync(ILifetimeScope lifetimeScope, Uri uri)
    {
        try
        {
            var appContext = lifetimeScope.Resolve<IAppContext>();
            await appContext.InvokeOnUIThreadAsync(async () =>
            {
                await lifetimeScope.Resolve<ITabManager>().NavigateAsync(uri);
            });
        }
        catch (Exception e)
        {
            lifetimeScope.Resolve<IAppLogger<StartupUriNavigationDispatcher>>().Error(e);
        }
    }
}

using ZYC.Framework.Modules.BlazorDemo.Abstractions;

namespace ZYC.Framework.Modules.BlazorDemo;

internal class BlazorServer : IBlazorServer
{
    private WebApplication? _app;

    private CancellationTokenSource? _cts;

    public BlazorServer(
        Uri baseUri,
        WebApplication app,
        CancellationTokenSource cancellationTokenSource)
    {
        BaseUri = baseUri;
        _app = app;
        _cts = cancellationTokenSource;
    }

    public Uri BaseUri { get; }

    public void Dispose()
    {
        var cts = Interlocked.Exchange(ref _cts, null);
        var app = Interlocked.Exchange(ref _app, null);

        try
        {
            _cts?.Cancel();
        }
        catch
        {
            //ignore
        }
        finally
        {
            cts?.Dispose();
        }


        // ReSharper disable once MethodSupportsCancellation
        _ = Task.Run(async () =>
        {
            try
            {
                if (app is not null)
                {
                    //!WARNING To prevent UI jams, skip <StopAsync> here
                    await app.DisposeAsync().ConfigureAwait(false);
                }
            }
            catch
            {
                //ignore
            }
        });
    }
}
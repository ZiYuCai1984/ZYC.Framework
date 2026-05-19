using System.IO;
using System.IO.Pipes;
using System.Text;

namespace ZYC.Framework;

internal sealed class StartupUriPipeServer : IDisposable
{
    private static readonly TimeSpan RestartDelay = TimeSpan.FromMilliseconds(250);

    private readonly CancellationTokenSource _cancellationTokenSource = new();

    private readonly Action<Uri> _handleUri;

    private readonly string _pipeName;

    private int _started;

    public StartupUriPipeServer(string pipeName, Action<Uri> handleUri)
    {
        _pipeName = pipeName;
        _handleUri = handleUri;
    }

    public void Start()
    {
        if (Interlocked.Exchange(ref _started, 1) == 1)
        {
            return;
        }

        _ = ListenAsync(_cancellationTokenSource.Token);
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    private async Task ListenAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ListenOnceAsync(cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch
            {
                await DelayBeforeRestartAsync(cancellationToken);
            }
        }
    }

    private async Task ListenOnceAsync(CancellationToken cancellationToken)
    {
        await using var pipe = new NamedPipeServerStream(
            _pipeName,
            PipeDirection.In,
            1,
            PipeTransmissionMode.Byte,
            PipeOptions.Asynchronous);

        await pipe.WaitForConnectionAsync(cancellationToken);

        using var reader = new StreamReader(pipe, Encoding.UTF8);
        var rawUri = await reader.ReadToEndAsync(cancellationToken);
        if (StartupUriParser.TryParse(rawUri, out var uri))
        {
            _handleUri(uri);
        }
    }

    private static async Task DelayBeforeRestartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(RestartDelay, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
    }
}

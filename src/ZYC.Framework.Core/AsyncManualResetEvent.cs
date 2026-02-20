namespace ZYC.Framework.Core;

/// <summary>
///     Async manual reset event: Set() => all waiters pass, Reset() => block.
/// </summary>
public sealed class AsyncManualResetEvent
{
    private volatile TaskCompletionSource<bool> _tcs = NewTcs(true);

    private static TaskCompletionSource<bool> NewTcs(bool initiallySet)
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (initiallySet)
        {
            tcs.TrySetResult(true);
        }

        return tcs;
    }

    public Task WaitAsync(CancellationToken ct)
    {
        return _tcs.Task.WaitAsync(ct);
    }

    public void Set()
    {
        _tcs.TrySetResult(true);
    }

    public void Reset()
    {
        while (true)
        {
            var tcs = _tcs;
            if (!tcs.Task.IsCompleted)
            {
                return;
            }

            var newTcs = NewTcs(false);
            if (Interlocked.CompareExchange(ref _tcs, newTcs, tcs) == tcs)
            {
                return;
            }
        }
    }
}
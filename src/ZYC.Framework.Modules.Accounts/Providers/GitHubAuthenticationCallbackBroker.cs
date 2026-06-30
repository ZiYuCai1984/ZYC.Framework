using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.Accounts.Providers;

[RegisterSingleInstance]
internal class GitHubAuthenticationCallbackBroker
{
    private readonly Dictionary<string, TaskCompletionSource<GitHubAuthenticationCallback>> _pendingCallbacks =
        new(StringComparer.Ordinal);

    private readonly object _syncRoot = new();

    public async Task<GitHubAuthenticationCallback> WaitAsync(
        string nonce,
        CancellationToken cancellationToken)
    {
        var taskCompletionSource =
            new TaskCompletionSource<GitHubAuthenticationCallback>(TaskCreationOptions.RunContinuationsAsynchronously);

        lock (_syncRoot)
        {
            if (!_pendingCallbacks.TryAdd(nonce, taskCompletionSource))
            {
                throw new InvalidOperationException("A GitHub authentication callback is already pending.");
            }
        }

        try
        {
            return await taskCompletionSource.Task.WaitAsync(cancellationToken);
        }
        finally
        {
            lock (_syncRoot)
            {
                _pendingCallbacks.Remove(nonce);
            }
        }
    }

    public bool TryComplete(GitHubAuthenticationCallback callback)
    {
        if (string.IsNullOrWhiteSpace(callback.Nonce))
        {
            return false;
        }

        TaskCompletionSource<GitHubAuthenticationCallback>? taskCompletionSource;
        lock (_syncRoot)
        {
            if (!_pendingCallbacks.Remove(callback.Nonce, out taskCompletionSource))
            {
                return false;
            }
        }

        taskCompletionSource.SetResult(callback);
        return true;
    }
}

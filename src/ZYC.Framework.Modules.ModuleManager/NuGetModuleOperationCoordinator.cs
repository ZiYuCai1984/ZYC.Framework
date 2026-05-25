using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.ModuleManager;

[RegisterSingleInstance]
internal sealed class NuGetModuleOperationCoordinator
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly object _syncRoot = new();

    private readonly AsyncLocal<bool> _operationOwnedByCurrentFlow = new();

    private bool _isExecuting;

    public bool IsExecuting
    {
        get
        {
            lock (_syncRoot)
            {
                return _isExecuting;
            }
        }
    }

    public event EventHandler? IsExecutingChanged;

    public async Task RunAsync(Func<Task> operation)
    {
        if (_operationOwnedByCurrentFlow.Value)
        {
            await operation();
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            _operationOwnedByCurrentFlow.Value = true;
            SetIsExecuting(true);
            await operation();
        }
        finally
        {
            SetIsExecuting(false);
            _operationOwnedByCurrentFlow.Value = false;
            _semaphore.Release();
        }
    }

    public async Task<TResult> RunAsync<TResult>(Func<Task<TResult>> operation)
    {
        if (_operationOwnedByCurrentFlow.Value)
        {
            return await operation();
        }

        await _semaphore.WaitAsync();
        try
        {
            _operationOwnedByCurrentFlow.Value = true;
            SetIsExecuting(true);
            return await operation();
        }
        finally
        {
            SetIsExecuting(false);
            _operationOwnedByCurrentFlow.Value = false;
            _semaphore.Release();
        }
    }

    public async Task<bool> TryRunAsync(Func<Task> operation)
    {
        if (_operationOwnedByCurrentFlow.Value)
        {
            await operation();
            return true;
        }

        if (!await _semaphore.WaitAsync(0))
        {
            return false;
        }

        try
        {
            _operationOwnedByCurrentFlow.Value = true;
            SetIsExecuting(true);
            await operation();
            return true;
        }
        finally
        {
            SetIsExecuting(false);
            _operationOwnedByCurrentFlow.Value = false;
            _semaphore.Release();
        }
    }

    private void SetIsExecuting(bool isExecuting)
    {
        lock (_syncRoot)
        {
            if (_isExecuting == isExecuting)
            {
                return;
            }

            _isExecuting = isExecuting;
        }

        IsExecutingChanged?.Invoke(this, EventArgs.Empty);
    }
}

using System.Diagnostics;

namespace ZYC.Framework.Core.Commands;

public class AsyncCommandBase<T> : AsyncCommandBase
{
    protected override Task InternalExecuteAsync(object? parameter)
    {
        return InternalExecuteAsync((T)parameter!);
    }

    protected virtual Task InternalExecuteAsync(T parameter)
    {
        return Task.CompletedTask;
    }

    [DebuggerHidden]
    protected override void InternalExecute(object? parameter)
    {
        throw new InvalidOperationException("override <InternalExecuteAsync>");
    }

    public override bool CanExecute(object? parameter)
    {
        return InternalCanExecute((T?)parameter);
    }

    protected virtual bool InternalCanExecute(T? parameter)
    {
        return true;
    }
}

public class AsyncCommandBase : CommandBase
{
    protected virtual Task InternalExecuteAsync(object? parameter)
    {
        return Task.CompletedTask;
    }

    // ReSharper disable once AsyncVoidMethod
    public override async void Execute(object? parameter)
    {
        await ExecuteAsync(parameter);
    }

    public async Task ExecuteAsync(object? parameter)
    {
        IsExecuting = true;
        try
        {
            await InternalExecuteAsync(parameter);
        }
        finally
        {
            IsExecuting = false;
        }

        RaiseExecuted();
    }

    [DebuggerHidden]
    protected override void InternalExecute(object? parameter)
    {
        throw new InvalidOperationException("override <InternalExecuteAsync>");
    }
}
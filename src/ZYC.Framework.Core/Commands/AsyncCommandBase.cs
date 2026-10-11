using System.Diagnostics;

namespace ZYC.Framework.Core.Commands;

public class AsyncCommandBase<T> : AsyncCommandBase
{
    protected override Task InternalExecuteAsync(object? parameter)
    {
        return parameter is T value
            ? InternalExecuteAsync(value)
            : Task.CompletedTask;
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
        //!WARNING WPF can pass a disconnected item while removing a bound container.
        return parameter is T value && InternalCanExecute(value);
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
        if (!CanExecute(parameter))
        {
            return;
        }

        try
        {
            IsExecuting = true;
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

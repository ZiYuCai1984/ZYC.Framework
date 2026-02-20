using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager;

internal sealed class ManagedTask : IManagedTask
{
    private readonly TaskManager _manager;

    private readonly TaskCompletionSource _tcs =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public ManagedTask(TaskManager manager, TaskRecord record)
    {
        _manager = manager;
        Snapshot = record;
    }

    public Guid Id => Snapshot.Id;

    public TaskRecord Snapshot { get; private set; }

    public Task StartAsync(CancellationToken ct = default)
    {
        return _manager.StartAsync(Id, ct);
    }

    public Task PauseAsync(CancellationToken ct = default)
    {
        return _manager.PauseAsync(Id, ct);
    }

    public Task ResumeAsync(CancellationToken ct = default)
    {
        return _manager.ResumeAsync(Id, ct);
    }

    public Task CancelAsync(CancellationToken ct = default)
    {
        return _manager.CancelAsync(Id, ct);
    }

    public Task Completion => _tcs.Task;

    internal void SetRecord(TaskRecord record)
    {
        Snapshot = record;
    }

    internal void TrySetCompleted()
    {
        _tcs.TrySetResult();
    }

    internal void TrySetFaulted(Exception ex)
    {
        _tcs.TrySetException(ex);
    }

    internal void TrySetCanceled()
    {
        _tcs.TrySetCanceled();
    }
}
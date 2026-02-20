namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when a task is enqueued.
/// </summary>
public sealed class ManagedTaskEnqueuedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskEnqueuedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskEnqueuedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
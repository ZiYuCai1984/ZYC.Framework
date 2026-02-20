namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when a task is canceled.
/// </summary>
public sealed class ManagedTaskCanceledEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskCanceledEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskCanceledEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
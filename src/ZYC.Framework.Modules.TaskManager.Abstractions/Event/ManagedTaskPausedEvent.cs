namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when a task is paused.
/// </summary>
public sealed class ManagedTaskPausedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskPausedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskPausedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
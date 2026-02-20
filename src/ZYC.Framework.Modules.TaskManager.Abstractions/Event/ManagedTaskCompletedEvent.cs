namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when a task completes successfully.
/// </summary>
public sealed class ManagedTaskCompletedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskCompletedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskCompletedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when task status text changes.
/// </summary>
public sealed class ManagedTaskStatusTextChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskStatusTextChangedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskStatusTextChangedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
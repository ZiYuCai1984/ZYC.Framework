namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when task creation fails.
/// </summary>
public sealed class ManagedTaskCreateFaultedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskCreateFaultedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskCreateFaultedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
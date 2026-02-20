namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when task creation fails.
/// </summary>
public sealed class ManagedCreateFaultedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedCreateFaultedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedCreateFaultedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
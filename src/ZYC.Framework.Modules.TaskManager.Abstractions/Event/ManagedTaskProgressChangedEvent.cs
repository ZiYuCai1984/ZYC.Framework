namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when task progress changes.
/// </summary>
public sealed class ManagedTaskProgressChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskProgressChangedEvent" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    public ManagedTaskProgressChangedEvent(TaskRecord snapshot)
    {
        Snapshot = snapshot;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }
}
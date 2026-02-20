namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when a task faults.
/// </summary>
public sealed class ManagedTaskFaultedEvnet
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskFaultedEvnet" /> class.
    /// </summary>
    /// <param name="snapshot">The task snapshot.</param>
    /// <param name="exception">The exception that caused the fault.</param>
    public ManagedTaskFaultedEvnet(TaskRecord snapshot, Exception exception)
    {
        Snapshot = snapshot;
        Exception = exception;
    }

    /// <summary>
    ///     Gets the task snapshot.
    /// </summary>
    public TaskRecord Snapshot { get; }

    /// <summary>
    ///     Gets the exception that caused the fault.
    /// </summary>
    public Exception Exception { get; }
}
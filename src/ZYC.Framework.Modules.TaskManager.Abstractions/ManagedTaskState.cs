namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Represents the lifecycle state of a managed task.
/// </summary>
public enum ManagedTaskState
{
    /// <summary>
    ///     Waiting in the queue.
    /// </summary>
    Queuing,

    /// <summary>
    ///     Currently running.
    /// </summary>
    Running,

    /// <summary>
    ///     Paused by request.
    /// </summary>
    Paused,

    /// <summary>
    ///     Completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    ///     Faulted with an error.
    /// </summary>
    Faulted,

    /// <summary>
    ///     Canceled by request.
    /// </summary>
    Canceled
}
namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Represents a managed task instance.
/// </summary>
public interface IManagedTask
{
    /// <summary>
    ///     Gets the unique task identifier.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    ///     Gets the current task snapshot.
    /// </summary>
    TaskRecord Snapshot { get; }

    /// <summary>
    ///     Gets a task that completes when execution finishes.
    /// </summary>
    Task Completion { get; }

    /// <summary>
    ///     Starts the task.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task StartAsync(CancellationToken ct = default);

    /// <summary>
    ///     Pauses the task.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task PauseAsync(CancellationToken ct = default);

    /// <summary>
    ///     Resumes the task.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task ResumeAsync(CancellationToken ct = default);

    /// <summary>
    ///     Cancels the task.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task CancelAsync(CancellationToken ct = default);
}
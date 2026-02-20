namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Provides context for task execution.
/// </summary>
public sealed class TaskExecutionContext
{
    /// <summary>
    ///     Gets the task identifier.
    /// </summary>
    public required Guid TaskId { get; init; }

    /// <summary>
    ///     Gets the pause token for the task.
    /// </summary>
    public required IPauseToken Pause { get; init; }

    /// <summary>
    ///     Gets the progress reporter.
    /// </summary>
    public IProgress<double>? Progress { get; init; }

    /// <summary>
    ///     Gets the status text reporter.
    /// </summary>
    public IProgress<string>? StatusText { get; init; }
}
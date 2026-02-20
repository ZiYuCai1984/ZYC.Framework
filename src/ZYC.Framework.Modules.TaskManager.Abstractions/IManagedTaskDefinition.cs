namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Defines how a managed task executes.
/// </summary>
public interface IManagedTaskDefinition
{
    /// <summary>
    ///     Gets the task type (e.g. "download", "build", "scan").
    /// </summary>
    string TaskType { get; }

    /// <summary>
    ///     Gets the display name.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///     Gets the optional description.
    /// </summary>
    string? Description { get; }

    /// <summary>
    ///     Executes the task.
    /// </summary>
    /// <param name="context">The execution context.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task ExecuteAsync(TaskExecutionContext context, CancellationToken ct);
}
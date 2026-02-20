namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Represents a pause token for task execution.
/// </summary>
public interface IPauseToken
{
    /// <summary>
    ///     Gets a value indicating whether execution is paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    ///     Waits while the execution is paused.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task WaitIfPausedAsync(CancellationToken ct);
}
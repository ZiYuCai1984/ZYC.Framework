namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Persists task records.
/// </summary>
public interface ITaskStore
{
    /// <summary>
    ///     Loads all task records.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The task records.</returns>
    Task<IReadOnlyList<TaskRecord>> LoadAllAsync(CancellationToken ct);

    /// <summary>
    ///     Saves all task records.
    /// </summary>
    /// <param name="records">The task records.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task SaveAllAsync(IReadOnlyList<TaskRecord> records, CancellationToken ct);
}
namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Provides task management operations.
/// </summary>
public interface ITaskManager
{
    /// <summary>
    ///     Initializes the task manager.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    internal Task InitializeAsync(CancellationToken ct = default);

    /// <summary>
    ///     Enqueues a new task.
    /// </summary>
    /// <param name="providerId">The provider identifier.</param>
    /// <param name="definitionId">The definition identifier.</param>
    /// <param name="version">The definition version.</param>
    /// <param name="payloadJson">The payload JSON.</param>
    /// <returns>The managed task.</returns>
    IManagedTask Enqueue(string providerId, string definitionId, int version, string payloadJson);

    /// <summary>
    ///     Gets all managed tasks.
    /// </summary>
    /// <returns>The task list.</returns>
    IReadOnlyList<IManagedTask> GetAllTasks();

    /// <summary>
    ///     Tries to find a managed task by identifier.
    /// </summary>
    /// <param name="id">The task identifier.</param>
    /// <param name="task">The resolved task.</param>
    /// <returns>True if the task was found; otherwise false.</returns>
    bool TryGetTask(Guid id, out IManagedTask task)
    {
        task = GetAllTasks().FirstOrDefault(t => t.Id == id)!;
        return task != null;
    }

    /// <summary>
    ///     Starts a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task StartAsync(Guid taskId, CancellationToken ct = default);

    /// <summary>
    ///     Pauses a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task PauseAsync(Guid taskId, CancellationToken ct = default);

    /// <summary>
    ///     Resumes a task.
    /// </summary>
    /// <param name="taskId">The task identifier.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The async operation.</returns>
    Task ResumeAsync(Guid taskId, CancellationToken ct = default);

    /// <summary>
    ///     Clears finished tasks and returns their identifiers.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>The cleared task identifiers.</returns>
    Task<Guid[]> ClearUpTasksAsync(CancellationToken ct = default);
}
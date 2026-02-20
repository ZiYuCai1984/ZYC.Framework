namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Represents a persisted task record.
/// </summary>
public sealed record TaskRecord
{
    /// <summary>
    ///     Gets the task identifier.
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    ///     Gets the provider identifier.
    /// </summary>
    public required string ProviderId { get; init; }

    /// <summary>
    ///     Gets the definition identifier.
    /// </summary>
    public required string DefinitionId { get; init; }

    /// <summary>
    ///     Gets the definition version.
    /// </summary>
    public required int DefinitionVersion { get; init; }

    /// <summary>
    ///     Gets the payload JSON.
    /// </summary>
    public required string PayloadJson { get; init; }

    /// <summary>
    ///     Gets the task state.
    /// </summary>
    public ManagedTaskState State { get; init; }

    /// <summary>
    ///     Gets the pause kind.
    /// </summary>
    public PauseKind PauseKind { get; init; }

    /// <summary>
    ///     Gets the creation time.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    ///     Gets the start time.
    /// </summary>
    public DateTimeOffset? StartedAt { get; init; }

    /// <summary>
    ///     Gets the end time.
    /// </summary>
    public DateTimeOffset? EndedAt { get; init; }

    /// <summary>
    ///     Gets the progress value.
    /// </summary>
    public double? Progress { get; init; }

    /// <summary>
    ///     Gets the status text.
    /// </summary>
    public string? StatusText { get; init; }

    /// <summary>
    ///     Gets the fault text.
    /// </summary>
    public string? FaultText { get; init; }
}
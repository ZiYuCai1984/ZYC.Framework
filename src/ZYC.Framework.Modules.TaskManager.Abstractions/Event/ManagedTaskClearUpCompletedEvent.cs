namespace ZYC.Framework.Modules.TaskManager.Abstractions.Event;

/// <summary>
///     Raised when task cleanup completes.
/// </summary>
public sealed class ManagedTaskClearUpCompletedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ManagedTaskClearUpCompletedEvent" /> class.
    /// </summary>
    /// <param name="taskIds">The cleared task identifiers.</param>
    public ManagedTaskClearUpCompletedEvent(Guid[] taskIds)
    {
        TaskIds = taskIds;
    }

    /// <summary>
    ///     Gets the cleared task identifiers.
    /// </summary>
    public Guid[] TaskIds { get; }
}
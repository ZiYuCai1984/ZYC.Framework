namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when the focus shifts between different workspaces.
/// </summary>
public sealed class WorkspaceFocusChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WorkspaceFocusChangedEvent" /> class.
    /// </summary>
    /// <param name="id">The unique identifier of the newly focused workspace, or <c>null</c> if focus is lost.</param>
    public WorkspaceFocusChangedEvent(Guid? id = null)
    {
        Id = id;
    }

    /// <summary>
    ///     Gets the unique identifier of the focused workspace.
    ///     Returns <c>null</c> if no workspace is currently focused.
    /// </summary>
    public Guid? Id { get; }
}
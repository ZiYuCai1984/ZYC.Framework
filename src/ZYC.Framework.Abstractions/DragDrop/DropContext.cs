namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Captures the environmental state and location when a drop event occurs.
/// </summary>
/// <param name="Target">The UI element or business object the payload was dropped onto.</param>
/// <param name="WorkspaceId">The unique identifier of the active workspace.</param>
/// <param name="Modifiers">The state of keyboard modifiers (Ctrl, Shift, etc.).</param>
/// <param name="ScreenPoint">The precise screen coordinates of the drop.</param>
/// <param name="CancellationToken">Allows for canceling asynchronous drop processing.</param>
public sealed record DropContext(
    object? Target,
    Guid WorkspaceId,
    ModifierKeys Modifiers,
    (double X, double Y) ScreenPoint,
    CancellationToken CancellationToken);
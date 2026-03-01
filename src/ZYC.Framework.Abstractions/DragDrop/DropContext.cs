namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
/// Captures the environmental state and location when a drop event occurs.
/// </summary>
public sealed record DropContext(
    object? Target,                 // The UI element or business object the payload was dropped onto.
    Guid WorkspaceId,               // The unique identifier of the active workspace.
    ModifierKeys Modifiers,         // The state of keyboard modifiers (Ctrl, Shift, etc.).
    (double X, double Y) ScreenPoint, // The precise screen coordinates of the drop.
    CancellationToken CancellationToken); // Allows for canceling asynchronous drop processing.
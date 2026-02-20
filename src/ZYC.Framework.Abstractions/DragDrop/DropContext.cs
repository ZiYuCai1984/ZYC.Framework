namespace ZYC.Framework.Abstractions.DragDrop;

public sealed record DropContext(
    object? Target,
    Guid WorkspaceId,
    ModifierKeys Modifiers,
    (double X, double Y) ScreenPoint,
    CancellationToken CancellationToken);
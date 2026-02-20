namespace ZYC.Framework.Abstractions.DragDrop;

public sealed record DropResolution(
    DropAction[] Actions,
    DropResolutionMode Mode,
    DropAction? DefaultAction);
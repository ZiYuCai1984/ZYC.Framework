namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Represents the result of resolving a drop operation.
/// </summary>
/// <param name="Actions">
///     The list of available actions that can be executed for the given drop.
///     May be empty if no handler can process the payload.
/// </param>
/// <param name="Mode">
///     Determines how the drop should be handled (e.g., no action, execute default, or require user selection).
/// </param>
/// <param name="DefaultAction">
///     The default action to execute when the resolution mode indicates automatic execution.
///     Can be <c>null</c> if no default action is applicable.
/// </param>
public sealed record DropResolution(
    DropAction[] Actions,
    DropResolutionMode Mode,
    DropAction? DefaultAction
);
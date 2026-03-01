namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Provides a way to discover available actions for a specific drop payload and context.
/// </summary>
public interface IDropActionProvider
{
    /// <summary>
    ///     Returns a list of compatible actions for the given drop.
    /// </summary>
    Task<DropAction[]> GetActionsAsync(DropPayload payload, DropContext context);
}
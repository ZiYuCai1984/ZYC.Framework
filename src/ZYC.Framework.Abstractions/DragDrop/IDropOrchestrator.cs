namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     The central coordinator responsible for determining the final resolution of a drop.
/// </summary>
public interface IDropOrchestrator
{
    /// <summary>
    ///     Analyzes the drop and decides which mode and actions should be applied.
    /// </summary>
    Task<DropResolution> ResolveAsync(DropPayload payload, DropContext context);
}
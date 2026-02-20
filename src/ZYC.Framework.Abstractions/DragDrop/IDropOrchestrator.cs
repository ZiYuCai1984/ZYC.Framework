namespace ZYC.Framework.Abstractions.DragDrop;

public interface IDropOrchestrator
{
    Task<DropResolution> ResolveAsync(DropPayload payload, DropContext context);
}
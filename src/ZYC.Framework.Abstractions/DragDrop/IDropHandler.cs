namespace ZYC.Framework.Abstractions.DragDrop;

public interface IDropHandler
{
    int Order { get; }
    
    bool CanHandle(DropPayload payload, DropContext context);

    Task HandleAsync(DropPayload payload, DropContext context);
}
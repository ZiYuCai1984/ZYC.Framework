namespace ZYC.Framework.Abstractions.DragDrop;

public interface IDropActionProvider
{
    Task<DropAction[]> GetActionsAsync(DropPayload payload, DropContext context);
}
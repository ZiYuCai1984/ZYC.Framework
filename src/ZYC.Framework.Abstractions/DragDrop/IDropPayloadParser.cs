namespace ZYC.Framework.Abstractions.DragDrop;

public interface IDropPayloadParser<in TDataObject>
{
    DropPayload Parse(TDataObject dataObject);
}
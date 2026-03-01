namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Parses raw data objects (e.g., from the OS or UI framework) into a unified <see cref="DropPayload" />.
/// </summary>
/// <typeparam name="TDataObject">The type of the raw data object (e.g., IDataObject).</typeparam>
public interface IDropPayloadParser<in TDataObject>
{
    /// <summary>
    ///     Parse the raw data
    /// </summary>
    /// <param name="dataObject"></param>
    /// <returns></returns>
    DropPayload Parse(TDataObject dataObject);
}
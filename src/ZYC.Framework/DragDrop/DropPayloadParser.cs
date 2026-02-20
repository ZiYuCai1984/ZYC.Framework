using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.DragDrop;

namespace ZYC.Framework.DragDrop;

[RegisterSingleInstanceAs(typeof(IDropPayloadParser<IDataObject>))]
internal class DropPayloadParser : IDropPayloadParser<IDataObject>
{
    public DropPayload Parse(IDataObject dataObject)
    {
        if (dataObject is null)
        {
            throw new ArgumentNullException(nameof(dataObject));
        }

        var paths = Array.Empty<string>();
        var extras = new Dictionary<string, object?>();

        // 1) File drop
        if (dataObject.GetDataPresent(DataFormats.FileDrop))
        {
            if (dataObject.GetData(DataFormats.FileDrop) is string[] raw)
            {
                paths = raw
                    .Where(p => !string.IsNullOrWhiteSpace(p))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();
            }
        }

        // 2) Text drop (optional)
        if (dataObject.GetDataPresent(DataFormats.UnicodeText))
        {
            var text = dataObject.GetData(DataFormats.UnicodeText) as string;
            if (!string.IsNullOrWhiteSpace(text))
            {
                extras["text"] = text;
            }
        }

        return new DropPayload(paths, extras);
    }
}
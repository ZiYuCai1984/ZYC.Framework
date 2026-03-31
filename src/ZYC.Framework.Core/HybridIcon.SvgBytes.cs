using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core;

public partial class HybridIcon
{
    private bool TrySetFromSvgBytes(byte[] buffer)
    {
        try
        {
            Content?.TryDispose();

            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = true
            };

            var reader = new FileSvgReader(settings);

            using var ms = new MemoryStream(buffer, false);
            var drawing = reader.Read(ms); // DrawingGroup
            if (drawing == null)
            {
                return false;
            }

            drawing.Freeze();

            var imgSource = new DrawingImage(drawing);
            imgSource.Freeze();

            var image = new Image
            {
                Stretch = Stretch.Uniform,
                Source = imgSource
            };

            Content = image;
            return true;
        }
        catch
        {
            return false;
        }
    }


    private static bool LooksLikeSvgXml(ReadOnlySpan<byte> bytes)
    {
        // UTF-8 BOM
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
        {
            bytes = bytes.Slice(3);
        }

        // skip leading whitespace
        var i = 0;
        while (i < bytes.Length && bytes[i] <= 0x20)
        {
            i++;
        }

        if (i >= bytes.Length)
        {
            return false;
        }

        // Very small sniff window (avoid large allocations)
        var head = bytes.Slice(i, Math.Min(256, bytes.Length - i));

        // Quick check: must contain '<'
        if (head.IndexOf((byte)'<') < 0)
        {
            return false;
        }

        // Decode head as UTF8 for contains check
        var text = Encoding.UTF8.GetString(head);
        return text.Contains("<svg", StringComparison.OrdinalIgnoreCase);
    }
}
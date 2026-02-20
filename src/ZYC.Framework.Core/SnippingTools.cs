using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ZYC.Framework.Core;

public static class SnippingTools
{
    public static byte[] StartScreenshot()
    {
        Process.Start(new ProcessStartInfo("ms-screenclip:") { UseShellExecute = true });

        if (!Clipboard.ContainsImage())
        {
            return [];
        }

        var image = Clipboard.GetImage();
        if (image == null)
        {
            return [];
        }

        using var ms = new MemoryStream();
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(image));
        encoder.Save(ms);
        return ms.ToArray();
    }
}
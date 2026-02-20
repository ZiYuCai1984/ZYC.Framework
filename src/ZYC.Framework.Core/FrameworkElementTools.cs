using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZYC.Framework.Core;

public static class FrameworkElementTools
{
    public static void SaveToBmp(FrameworkElement visual, string fileName)
    {
        var encoder = new BmpBitmapEncoder();
        SaveUsingEncoder(visual, fileName, encoder);
    }

    public static void SaveToPng(FrameworkElement visual, string fileName)
    {
        var encoder = new PngBitmapEncoder();
        SaveUsingEncoder(visual, fileName, encoder);
    }

    public static void SaveToJpeg(FrameworkElement visual, string fileName)
    {
        var encoder = new JpegBitmapEncoder();
        SaveUsingEncoder(visual, fileName, encoder);
    }

    private static void SaveUsingEncoder(FrameworkElement visual, string fileName, BitmapEncoder encoder)
    {
        var bitmap = new RenderTargetBitmap((int)visual.ActualWidth, (int)visual.ActualHeight, 96, 96,
            PixelFormats.Pbgra32);
        var visualSize = new Size(visual.ActualWidth, visual.ActualHeight);
        visual.Measure(visualSize);
        visual.Arrange(new Rect(visualSize));
        bitmap.Render(visual);
        var frame = BitmapFrame.Create(bitmap);
        encoder.Frames.Add(frame);

        using var stream = File.Create(fileName);
        encoder.Save(stream);
    }
}
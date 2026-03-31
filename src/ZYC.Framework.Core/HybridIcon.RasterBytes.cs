using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core;

public partial class HybridIcon
{
    private bool TrySetFromRasterBytes(byte[] buffer)
    {
        try
        {
            Content?.TryDispose();

            var image = new Image { Stretch = Stretch.Uniform };

            var bitmap = new BitmapImage();
            using (var ms = new MemoryStream(buffer, false))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
            }

            bitmap.Freeze();

            image.Source = bitmap;
            Content = image;
            return true;
        }
        catch
        {
            return false;
        }
    }
}
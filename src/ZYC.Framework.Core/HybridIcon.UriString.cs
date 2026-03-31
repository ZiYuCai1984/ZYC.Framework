using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core;

public partial class HybridIcon
{
    private bool TrySetFromUriString(string s)
    {
        try
        {
            Uri? uri = null;
            if (Uri.TryCreate(s, UriKind.Absolute, out var absoluteUri))
            {
                uri = absoluteUri;
            }
            else if (File.Exists(s))
            {
                uri = new Uri(Path.GetFullPath(s), UriKind.Absolute);
            }
            else if (Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var packUri)
                     && packUri.IsAbsoluteUri
                     && string.Equals(packUri.Scheme, "pack", StringComparison.OrdinalIgnoreCase))
            {
                uri = packUri;
            }

            if (uri == null)
            {
                return false;
            }

            Content?.TryDispose();

            var image = new Image { Stretch = Stretch.Uniform };
            image.ImageFailed += (_, _) => SetFromMaterialIcon(DefaultIconKind);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = uri;
            if (uri.IsFile)
            {
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
            }

            bitmap.EndInit();

            if (uri.IsFile && bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }

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
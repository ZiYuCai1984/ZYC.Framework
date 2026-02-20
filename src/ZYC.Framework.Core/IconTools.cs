using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Core;

public static class IconTools
{
    private static Icon? _icon;

    private static ImageSource? _iconImageSource;

    public static Icon CurrentProcessIcon
    {
        get
        {
            _icon ??= Icon.ExtractAssociatedIcon(ProcessTools.GetCurrentExeFullName());

            Debug.Assert(_icon != null);
            return _icon;
        }
    }


    public static ImageSource IconImageSource
    {
        get
        {
            _iconImageSource ??= Imaging.CreateBitmapSourceFromHIcon(
                CurrentProcessIcon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            Debug.Assert(_iconImageSource != null);

            return _iconImageSource;
        }
    }
}
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

    /// <summary>
    ///     Gets the full path of the icon file set by <see cref="SetOverrideIcon" />,
    ///     or <c>null</c> when the icon embedded in the executable is used.
    /// </summary>
    public static string? OverrideIconPath { get; private set; }

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

    /// <summary>
    ///     Replaces the application icon with the specified <c>.ico</c> file.
    ///     Call it before any window or tray icon reads <see cref="CurrentProcessIcon" /> or <see cref="IconImageSource" />.
    /// </summary>
    /// <param name="iconFilePath">The full path of the <c>.ico</c> file.</param>
    public static void SetOverrideIcon(string iconFilePath)
    {
        var icon = new Icon(iconFilePath);

        BitmapFrame imageSource;
        try
        {
            imageSource = BitmapFrame.Create(
                new Uri(iconFilePath, UriKind.Absolute),
                BitmapCreateOptions.None,
                BitmapCacheOption.OnLoad);

            if (imageSource.CanFreeze)
            {
                imageSource.Freeze();
            }
        }
        catch
        {
            icon.Dispose();
            throw;
        }

        _icon = icon;
        _iconImageSource = imageSource;
        OverrideIconPath = iconFilePath;
    }
}
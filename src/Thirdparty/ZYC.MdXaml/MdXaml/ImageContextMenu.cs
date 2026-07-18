using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace ZYC.MdXaml
{
    /// <summary>
    ///     Attaches a "Copy Image" / "Save Image As..." context menu to rendered images.
    /// </summary>
    internal static class ImageContextMenu
    {
        private const int MaxRenderDimension = 4096;

        public static void Attach(Image image, string? sourceHint)
        {
            var copyItem = new MenuItem { Header = "Copy Image" };
            copyItem.Click += (s, e) => CopyImage(image);

            var saveItem = new MenuItem { Header = "Save Image As..." };
            saveItem.Click += (s, e) => SaveImage(image, sourceHint);

            var menu = new ContextMenu();
            menu.Opened += (s, e) =>
            {
                var hasSource = image.Source is not null;
                copyItem.IsEnabled = hasSource;
                saveItem.IsEnabled = hasSource;
            };
            menu.Items.Add(copyItem);
            menu.Items.Add(saveItem);

            image.ContextMenu = menu;
        }

        private static void CopyImage(Image image)
        {
            try
            {
                var bitmap = ToBitmapSource(image.Source);
                if (bitmap is not null)
                {
                    Clipboard.SetImage(bitmap);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[IMG] copy image failed: " + ex);
            }
        }

        private static void SaveImage(Image image, string? sourceHint)
        {
            try
            {
                var bitmap = ToBitmapSource(image.Source);
                if (bitmap is null)
                {
                    return;
                }

                var dialog = new SaveFileDialog
                {
                    Filter = "PNG Image|*.png|All Files|*.*",
                    DefaultExt = ".png",
                    FileName = BuildFileName(sourceHint),
                };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                using var stream = File.Create(dialog.FileName);
                encoder.Save(stream);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("[IMG] save image failed: " + ex);
                MessageBox.Show(ex.Message, "Save Image As", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static BitmapSource? ToBitmapSource(ImageSource? source)
        {
            switch (source)
            {
                case null:
                    return null;

                case BitmapSource bitmapSource:
                    return bitmapSource;

                default:
                {
                    // DrawingImage (e.g. rendered SVG): rasterize at natural size.
                    var width = ClampDimension(source.Width);
                    var height = ClampDimension(source.Height);

                    var visual = new DrawingVisual();
                    using (var context = visual.RenderOpen())
                    {
                        context.DrawImage(source, new Rect(0, 0, width, height));
                    }

                    var target = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                    target.Render(visual);
                    target.Freeze();
                    return target;
                }
            }
        }

        private static int ClampDimension(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value) || value < 1)
            {
                return 1;
            }

            return (int)Math.Min(Math.Ceiling(value), MaxRenderDimension);
        }

        private static string BuildFileName(string? sourceHint)
        {
            var name = "";

            try
            {
                if (!string.IsNullOrWhiteSpace(sourceHint) &&
                    !sourceHint.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var path = sourceHint;
                    if (Uri.TryCreate(sourceHint, UriKind.Absolute, out var uri))
                    {
                        path = uri.AbsolutePath;
                    }

                    name = Path.GetFileNameWithoutExtension(path);
                    foreach (var invalid in Path.GetInvalidFileNameChars())
                    {
                        name = name.Replace(invalid, '_');
                    }
                }
            }
            catch
            {
                name = "";
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "image";
            }

            return name + ".png";
        }
    }
}

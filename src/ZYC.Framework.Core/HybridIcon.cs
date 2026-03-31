using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.IconPacks;

namespace ZYC.Framework.Core;

public partial class HybridIcon : ContentControl
{
    public static readonly DependencyProperty IconProperty
        = DependencyProperty.Register(nameof(Icon),
            typeof(string), typeof(HybridIcon),
            new PropertyMetadata(DefaultIcon, OnIconChanged));

    public static PackIconMaterialKind DefaultIconKind { get; set; } = PackIconMaterialKind.Bug;
    public static string DefaultIcon => DefaultIconKind.ToString();

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var hybridIcon = (HybridIcon)d;

        if (e.NewValue == null)
        {
            hybridIcon.SetFromMaterialIcon(DefaultIconKind);
            return;
        }

        var s = (e.NewValue.ToString() ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(s))
        {
            hybridIcon.SetFromMaterialIcon(DefaultIconKind);
            return;
        }


        // 1) Try Material icon name first
        if (Enum.TryParse<PackIconMaterialKind>(s, true, out var materialKind))
        {
            hybridIcon.SetFromMaterialIcon(materialKind);
            return;
        }

        // 2) Try Base64 (optionally Data URI)
        if (TryDecodeBase64Payload(s, out var buffer, out var mime))
        {
            // SVG first
            if (string.Equals(mime, "image/svg+xml", StringComparison.OrdinalIgnoreCase) || LooksLikeSvgXml(buffer))
            {
                if (hybridIcon.TrySetFromSvgBytes(buffer))
                {
                    return;
                }

                if (hybridIcon.TrySetFromRasterBytes(buffer))
                {
                    return;
                }

                hybridIcon.SetFromMaterialIcon(DefaultIconKind);
                return;
            }

            // Not SVG -> raster
            if (hybridIcon.TrySetFromRasterBytes(buffer))
            {
                return;
            }

            hybridIcon.SetFromMaterialIcon(DefaultIconKind);
            return;
        }

        // 3) Try image URI or local file path.
        if (hybridIcon.TrySetFromUriString(s))
        {
            return;
        }

        // 4) Emoji / emoji sequence.
        if (LooksLikeEmoji(s))
        {
            hybridIcon.SetFromEmoji(s);
            return;
        }

        // 5) Fallback
        hybridIcon.SetFromMaterialIcon(DefaultIconKind);
    }


    private static bool TryDecodeBase64Payload(string s, out byte[] bytes, out string? mime)
    {
        bytes = Array.Empty<byte>();
        mime = null;

        // Support: data:image/svg+xml;base64,xxxx
        // Support: pure base64 (xxxx)
        var base64 = s;

        if (s.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
        {
            var comma = s.IndexOf(',');
            if (comma <= 0 || comma >= s.Length - 1)
            {
                return false;
            }

            var meta = s.Substring(5, comma - 5); // after "data:"
            base64 = s.Substring(comma + 1);

            // meta: "image/svg+xml;base64"
            var semi = meta.IndexOf(';');
            mime = (semi > 0 ? meta.Substring(0, semi) : meta).Trim();

            if (!meta.Contains("base64", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        var maxLen = (base64.Length + 3) / 4 * 3;
        var buffer = new byte[maxLen];

        if (!Convert.TryFromBase64String(base64, buffer, out var written))
        {
            return false;
        }

        if (written != buffer.Length)
        {
            Array.Resize(ref buffer, written);
        }

        bytes = buffer;
        return true;
    }
}
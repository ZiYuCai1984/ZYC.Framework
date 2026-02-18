using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.IconPacks;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using ZYC.CoreToolkit;
using WTextBlock = Emoji.Wpf.TextBlock;

namespace ZYC.Automation.Core;

public class HybridIcon : ContentControl
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

        // 3) Emoji / emoji sequence.
        if (LooksLikeEmoji(s))
        {
            hybridIcon.SetFromEmoji(s);
            return;
        }

        // 4) Fallback
        hybridIcon.SetFromMaterialIcon(DefaultIconKind);
    }

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

        //if (!LooksLikeBase64(base64))
        //{
        //    return false;
        //}

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

    private void SetFromMaterialIcon(PackIconMaterialKind kind)
    {
        Content?.TryDispose();

        Content = new PackIconMaterial
        {
            Kind = kind
        };

        if (Content is PackIconMaterial pi)
        {
            pi.SetBinding(VerticalAlignmentProperty, new Binding(nameof(VerticalAlignment)) { Source = this });
            pi.SetBinding(VerticalContentAlignmentProperty,
                new Binding(nameof(VerticalContentAlignment)) { Source = this });

            pi.SetBinding(HorizontalAlignmentProperty, new Binding(nameof(HorizontalAlignment)) { Source = this });
            pi.SetBinding(HorizontalContentAlignmentProperty,
                new Binding(nameof(HorizontalContentAlignment)) { Source = this });


            pi.SetBinding(WidthProperty, new Binding(nameof(Width)) { Source = this });
            pi.SetBinding(HeightProperty, new Binding(nameof(Height)) { Source = this });
            pi.SetBinding(ForegroundProperty, new Binding(nameof(Foreground)) { Source = this });
            pi.SetBinding(FontSizeProperty, new Binding(nameof(FontSize)) { Source = this });
        }
    }

    private void SetFromEmoji(string emojiText)
    {
        Content?.TryDispose();

        var tb = new WTextBlock
        {
            Text = emojiText,
            TextAlignment = TextAlignment.Center
        };


        tb.SetBinding(VerticalAlignmentProperty, new Binding(nameof(VerticalAlignment)) { Source = this });
        tb.SetBinding(VerticalContentAlignmentProperty,
            new Binding(nameof(VerticalContentAlignment)) { Source = this });

        tb.SetBinding(HorizontalAlignmentProperty, new Binding(nameof(HorizontalAlignment)) { Source = this });
        tb.SetBinding(HorizontalContentAlignmentProperty,
            new Binding(nameof(HorizontalContentAlignment)) { Source = this });

        tb.SetBinding(FontSizeProperty, new Binding(nameof(FontSize)) { Source = this });
        tb.SetBinding(ForegroundProperty, new Binding(nameof(Foreground)) { Source = this });

        tb.SetBinding(FontFamilyProperty, new Binding(nameof(FontFamily)) { Source = this });


        TextOptions.SetTextFormattingMode(tb, TextFormattingMode.Display);
        TextOptions.SetTextRenderingMode(tb, TextRenderingMode.Auto);

        Content = tb;
    }

    // ----------------- Helpers -----------------

    private static bool LooksLikeBase64(string s)
    {
        // Rough check: length is multiple of 4 and contains only Base64 chars to reduce pointless decoding attempts.
        if (s.Length < 8 || s.Length % 4 != 0)
        {
            return false;
        }

        foreach (var c in s)
        {
            var ok =
                (c >= 'A' && c <= 'Z') ||
                (c >= 'a' && c <= 'z') ||
                (c >= '0' && c <= '9') ||
                c == '+' || c == '/' || c == '=';
            if (!ok)
            {
                return false;
            }
        }

        return true;
    }

    private static bool LooksLikeEmoji(string s)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            return false;
        }

        // Allow complex sequences: ZWJ (200D), variation selectors (FE0F), skin tones (1F3FB..1F3FF),
        // composite flags, etc. Any emoji code point in the sequence qualifies as emoji.
        var e = StringInfo.GetTextElementEnumerator(s);
        while (e.MoveNext())
        {
            var element = e.GetTextElement();
            if (ContainsEmojiCodePoint(element))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsEmojiCodePoint(string textElement)
    {
        // Walk code points in the text element.
        for (var i = 0; i < textElement.Length; i++)
        {
            var codePoint = char.IsSurrogatePair(textElement, i)
                ? char.ConvertToUtf32(textElement, i++)
                : textElement[i];

            // Common emoji ranges (not exhaustive but covers most cases).
            if (IsEmojiCodePoint(codePoint))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsEmojiCodePoint(int cp)
    {
        // ---- Common emoji ranges ----
        if ((cp >= 0x1F300 && cp <= 0x1F5FF) || // Misc Symbols & Pictographs
            (cp >= 0x1F600 && cp <= 0x1F64F) || // Emoticons
            (cp >= 0x1F680 && cp <= 0x1F6FF) || // Transport & Map
            (cp >= 0x1F700 && cp <= 0x1F77F) || // Alchemical Symbols (some with emoji style)
            (cp >= 0x1F780 && cp <= 0x1F7FF) || // Geometric Shapes Extended
            (cp >= 0x1F800 && cp <= 0x1F8FF) || // Supplemental Arrows-C (some emoji)
            (cp >= 0x1F900 && cp <= 0x1F9FF) || // Supplemental Symbols & Pictographs
            (cp >= 0x1FA70 && cp <= 0x1FAFF) || // Symbols & Pictographs Extended-A
            (cp >= 0x2600 && cp <= 0x26FF) || // Misc Symbols
            (cp >= 0x2700 && cp <= 0x27BF) || // Dingbats
            (cp >= 0x1F1E6 && cp <= 0x1F1FF)) // Regional Indicators (flags)
        {
            return true;
        }

        // ---- Special control characters ----
        if (cp == 0x200D || // Zero Width Joiner (ZWJ)
            cp == 0xFE0F) // Variation Selector-16 (emoji presentation)
        {
            return true;
        }

        // ---- Fallback: rely on Unicode category ----
        // This catches some emoji-like symbols such as Misc Symbols/Dingbats.
        var uc = CharUnicodeInfo.GetUnicodeCategory(char.ConvertFromUtf32(cp), 0);
        if (uc == UnicodeCategory.OtherSymbol)
        {
            return true;
        }

        return false;
    }
}
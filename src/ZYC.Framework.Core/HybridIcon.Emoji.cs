using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ZYC.CoreToolkit;
using WTextBlock = Emoji.Wpf.TextBlock;

namespace ZYC.Framework.Core;

public partial class HybridIcon
{
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
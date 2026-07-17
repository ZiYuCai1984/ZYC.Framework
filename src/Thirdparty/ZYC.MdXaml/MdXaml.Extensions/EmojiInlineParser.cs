using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Documents;
using Emoji.Wpf;
using ZYC.MdXaml.Plugins;

namespace ZYC.MdXaml.Extensions;

internal sealed class EmojiInlineParser : IInlineParser
{
    // Match "possible emoji start" (avoid matching plain digits; keycap branch requires 20E3 ending).
    private static readonly Regex First =
        new Regex(
            @"(?:
                (?:[\u0023\u002A\u0030-\u0039]\uFE0F?\u20E3)         # keycap: # * 0-9 + VS16? + 20E3
              | [\u00A9\u00AE\u203C\u2049\u2122\u2139\u3030\u303D\u3297\u3299\u2763\u2764]
              | [\u231A-\u231B\u23E9-\u23F3\u23F8-\u23FA\u23CF]
              | [\u25AA-\u25AB\u25B6\u25C0\u25FB-\u25FE]
              | [\u2600-\u26FF]
              | [\u2700-\u27BF]
              | [\u2934-\u2935]
              | [\u2B05-\u2B55]
              | \u24C2
              | \uD83C[\uDC00-\uDFFF]                                # ★ Includes 🆕 and other Enclosed/Flags
              | \uD83D[\uDC00-\uDFFF]
              | \uD83E[\uDC00-\uDFFF]                                # 1F800–1FBFF
            )",
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace
#if NET7_0_OR_GREATER
            | RegexOptions.NonBacktracking
#endif
        );

    public Regex FirstMatchPattern => First;

    public IEnumerable<Inline> Parse(string text, Match firstMatch, IMarkdown engine,
        out int parseTextBegin, out int parseTextEnd)
    {
        int i = firstMatch.Index;
        int j = firstMatch.Index + firstMatch.Length;

        // Pair regional indicators so country flags render as one glyph.
        if (j - i == 2 && IsRegionalIndicator(text, i) && IsRegionalIndicator(text, j))
        {
            j += 2;
        }

        // Expand the matched "start" into a full emoji sequence.
        ExpandEmojiCluster(text, ref j);

        parseTextBegin = i;
        parseTextEnd   = j;

        var emoji = new TextBlock
        {
            Text = text.Substring(i, j - i),
            //FontSize = engine is FrameworkElement fe ? fe.FontSize : 16,
            VerticalAlignment = VerticalAlignment.Center
        };
        // (Optional) Bind style: let color/font size follow the host.
        //emoji.SetBinding(TextBlock.FontSizeProperty, new Binding(nameof(TextElement.FontSize)) { Source = fe });
        //emoji.SetBinding(TextBlock.ForegroundProperty, new Binding(nameof(TextElement.Foreground)) { Source = fe });

        return new[] { new InlineUIContainer(emoji) { BaselineAlignment = BaselineAlignment.TextBottom } };
    }

    private static void ExpandEmojiCluster(string s, ref int j)
    {
        // Allow appending: VS16/VS15, skin tone modifiers, ZWJ chain, tag sequence (subdivision flags), trailing VS16.
        ConsumeVS(s, ref j);                    // FE0F/FE0E
        ConsumeSkinTone(s, ref j);              // 1F3FB..1F3FF

        // ZWJ sequence: (... ZWJ base [VS] [skin])*
        while (Peek(s, j) == 0x200D)
        {
            int k = j + 1;
            if (!TryConsumeEmojiBase(s, ref k)) break; // if base is missing, do not consume ZWJ
            j = k;
            ConsumeVS(s, ref j);
            ConsumeSkinTone(s, ref j);
        }

        // Tag subdivision flags: U+E0020..E007E, optionally ending with U+E007F.
        while (TryConsumeTagChar(s, ref j)) { /* loop */ }
        TryConsumeCancelTag(s, ref j);

        // Some platforms allow another trailing VS16.
        ConsumeVS(s, ref j);
    }

    private static void ConsumeVS(string s, ref int j)
    {
        var cp = Peek(s, j);
        if (cp == 0xFE0F || cp == 0xFE0E) j += 1;
    }

    private static void ConsumeSkinTone(string s, ref int j)
    {
        // U+1F3FB..U+1F3FF => surrogate: \uD83C\uDFFB .. \uD83C\uDFFF
        if (j + 1 < s.Length && s[j] == '\uD83C' && s[j + 1] >= '\uDFFB' && s[j + 1] <= '\uDFFF')
            j += 2;
    }

    private static bool TryConsumeEmojiBase(string s, ref int j)
    {
        if (j >= s.Length) return false;

        // Keycap base: [#*0-9] FE0F? 20E3
        if (IsKeycapBase(s, j, out int len))
        {
            j += len;
            return true;
        }

        char c = s[j];

        // BMP range
        if (IsBmpEmojiBase(c))
        {
            j += 1;
            return true;
        }

        // High surrogate D83C/D83D/D83E + low surrogate
        if (j + 1 < s.Length)
        {
            char h = s[j], l = s[j + 1];
            if ((h == '\uD83C' || h == '\uD83D' || h == '\uD83E') && l >= '\uDC00' && l <= '\uDFFF')
            {
                j += 2;
                return true;
            }
        }

        return false;
    }

    private static bool IsKeycapBase(string s, int i, out int len)
    {
        len = 0;
        if (i >= s.Length) return false;

        char c = s[i];
        bool isBase = c == '#' || c == '*' || (c >= '0' && c <= '9');
        if (!isBase) return false;

        int j = i + 1;
        if (j < s.Length && (s[j] == '\uFE0F' || s[j] == '\uFE0E')) j++;

        if (j < s.Length && s[j] == '\u20E3')
        {
            len = j - i + 1;
            return true;
        }
        return false;
    }

    private static bool IsBmpEmojiBase(char c)
    {
        return
            c == '\u24C2' || c == '\u3030' || c == '\u303D' || c == '\u3297' || c == '\u3299' ||
            c == '\u00A9' || c == '\u00AE' || c == '\u203C' || c == '\u2049' || c == '\u2122' || c == '\u2139' ||
            (c >= '\u231A' && c <= '\u231B') || c == '\u23CF' ||
            (c >= '\u23E9' && c <= '\u23F3') || (c >= '\u23F8' && c <= '\u23FA') ||
            (c >= '\u25AA' && c <= '\u25AB') || c == '\u25B6' || c == '\u25C0' || (c >= '\u25FB' && c <= '\u25FE') ||
            (c >= '\u2600' && c <= '\u26FF') ||
            (c >= '\u2700' && c <= '\u27BF') ||
            (c >= '\u2934' && c <= '\u2935') ||
            (c >= '\u2B05' && c <= '\u2B55') ||
            c == '\u2763' || c == '\u2764';
    }

    private static bool IsRegionalIndicator(string s, int i)
    {
        // Regional indicator A..Z: U+1F1E6..U+1F1FF => high surrogate D83C + DDE6..DDFF
        return i + 1 < s.Length && s[i] == '\uD83C' && s[i + 1] >= '\uDDE6' && s[i + 1] <= '\uDDFF';
    }

    private static bool TryConsumeTagChar(string s, ref int j)
    {
        // Plane 14 tags: U+E0020..U+E007E => surrogate: \uDB40\uDC20 .. \uDB40\uDC7E
        if (j + 1 < s.Length && s[j] == '\uDB40' && s[j + 1] >= '\uDC20' && s[j + 1] <= '\uDC7E')
        {
            j += 2;
            return true;
        }
        return false;
    }

    private static void TryConsumeCancelTag(string s, ref int j)
    {
        // U+E007F => \uDB40\uDC7F
        if (j + 1 < s.Length && s[j] == '\uDB40' && s[j + 1] == '\uDC7F')
            j += 2;
    }

    private static int Peek(string s, int i)
        => (i < s.Length) ? s[i] : -1;
}

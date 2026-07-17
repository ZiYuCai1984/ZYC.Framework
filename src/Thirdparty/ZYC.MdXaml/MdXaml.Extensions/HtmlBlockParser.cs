using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using ZYC.MdXaml.Plugins;

namespace ZYC.MdXaml.MdXaml.Extensions;

internal sealed class HtmlBlockParser : IBlockParser
{
    // Block tags we support: p, div, h1-h6, blockquote, pre, hr, ul, ol, li, table, thead, tbody, tr, th, td, details, summary
    // Must start at line start (optionally indented).
    public Regex FirstMatchPattern { get; } =
        new(
            @"(?m)^[ \t]*<(?:(?:p|div|blockquote|pre|hr|ul|ol|li|table|thead|tbody|tr|th|td|details|summary|h[1-6])\b|!--|!\[CDATA\[|!\s*DOCTYPE\b|\?)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public IEnumerable<Block> Parse(
        string text, Match firstMatch, bool supportTextAlignment,
        IMarkdown engine,
        out int parseTextBegin, out int parseTextEnd)
    {
        // Include indentation before '<' if present (since regex anchors line start)
        parseTextBegin = firstMatch.Index;
        parseTextEnd = firstMatch.Index;

        // Find the actual '<' position (regex includes optional whitespace)
        var lt = text.IndexOf('<', firstMatch.Index);
        if (lt < 0)
        {
            return null!;
        }

        if (!HtmlFragmentExtractor.TryExtractElementOrComment(text, lt, out var end))
        {
            return null!;
        }

        // Consume trailing newline(s) after a block element if present (nice for block parsing)
        var consumeEnd = end;
        while (consumeEnd < text.Length && (text[consumeEnd] == '\r' || text[consumeEnd] == '\n'))
        {
            // consume at most one blank line worth
            if (text[consumeEnd] == '\n')
            {
                consumeEnd++;
                break;
            }

            consumeEnd++;
        }

        var html = text.Substring(lt, end - lt);
        parseTextEnd = consumeEnd;

        var root = HtmlDom.ParseAsWrapper(html);

        var markdownHtmlContext = new MarkdownHtmlContext(engine);

        HtmlDomSanitizer.Sanitize(root, markdownHtmlContext);
        var renderer = new WpfHtmlRenderer(markdownHtmlContext)
        {
            SupportTextAlignment = supportTextAlignment
        };

        // Comments, doctypes and fully-rejected content yield no blocks;
        // still consume the text so the raw markup is not shown as a paragraph.
        return renderer.RenderBlocks(root).ToArray();
    }
}
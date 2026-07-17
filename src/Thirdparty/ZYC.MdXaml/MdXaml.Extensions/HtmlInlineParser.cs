using System.Text.RegularExpressions;
using System.Windows.Documents;
using ZYC.MdXaml.Plugins;

namespace ZYC.MdXaml.MdXaml.Extensions;

internal sealed class HtmlInlineParser : IInlineParser
{
    // Inline tags we support: a, img, br, strong/b, em/i, del/s/strike, code, kbd, sub, sup, span, input[type=checkbox]
    public Regex FirstMatchPattern { get; } =
        new(@"<(?:(?:a|img|br|strong|b|em|i|del|s|strike|code|kbd|sub|sup|span|input)\b)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public IEnumerable<Inline> Parse(
        string text, Match firstMatch,
        IMarkdown engine,
        out int parseTextBegin, out int parseTextEnd)
    {
        parseTextBegin = firstMatch.Index;
        parseTextEnd = firstMatch.Index;

        if (!HtmlFragmentExtractor.TryExtractElementOrComment(text, firstMatch.Index, out var end))
        {
            return null!;
        }

        var html = text.Substring(firstMatch.Index, end - firstMatch.Index);
        parseTextEnd = end;

        // Parse fragment DOM
        var root = HtmlDom.ParseAsWrapper(html);


        var markdownHtmlContext = new MarkdownHtmlContext(engine);

        HtmlDomSanitizer.Sanitize(root, markdownHtmlContext);

        // Render Inlines from wrapper children.
        // Sanitized-away content yields no inlines; still consume the text
        // so the raw markup is not shown literally.
        var renderer = new WpfHtmlRenderer(markdownHtmlContext);
        return renderer.RenderInlines(root).ToArray();
    }
}
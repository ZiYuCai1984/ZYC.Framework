using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace ZYC.MdXaml.MdXaml.Extensions;

internal static class HtmlDom
{
    public static IElement ParseAsWrapper(string html)
    {
        var parser = new HtmlParser();

        // Fragment parsing (innerHTML semantics): unlike string-wrapping the
        // fragment in a "<div>", stray end tags cannot close the wrapper and
        // silently drop the remaining content.
        var document = parser.ParseDocument(string.Empty);
        var wrapper = document.CreateElement("div");

        var nodes = parser.ParseFragment(html, wrapper);
        foreach (var node in nodes.ToArray())
        {
            wrapper.AppendChild(node);
        }

        return wrapper;
    }
}

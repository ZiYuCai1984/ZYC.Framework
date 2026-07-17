using System.Windows.Input;
using ZYC.MdXaml.Plugins;

namespace ZYC.MdXaml.MdXaml.Extensions;

internal class MarkdownHtmlContext : IMarkdownHtmlContext
{
    public MarkdownHtmlContext(IMarkdown markdown)
    {
        Markdown = markdown;
    }

    private IMarkdown Markdown { get; }

    public Uri? BaseUri => Markdown.BaseUri;

    public bool AllowDataImages => true;

    public ICommand? HyperlinkCommand => Markdown.HyperlinkCommand;

    public HyperLinkClickCallback? OnHyperLinkClicked
        => Markdown is ZYC.MdXaml.Markdown engine ? engine.OnHyperLinkClicked : null;
}

using System.Windows.Input;

namespace ZYC.MdXaml.MdXaml.Extensions;

/// <summary>
///     Optional context your engine can implement to help resolve relative URLs.
/// </summary>
internal interface IMarkdownHtmlContext
{
    Uri? BaseUri { get; }
    bool AllowDataImages { get; } // allow data:image/... for img src

    ICommand? HyperlinkCommand { get; }
    HyperLinkClickCallback? OnHyperLinkClicked { get; }
}

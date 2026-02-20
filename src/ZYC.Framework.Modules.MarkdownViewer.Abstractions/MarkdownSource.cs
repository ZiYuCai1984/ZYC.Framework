namespace ZYC.Framework.Modules.MarkdownViewer.Abstractions;

public class MarkdownSource
{
    public MarkdownSource(Uri sourceUri, Uri baseUri)
    {
        SourceUri = sourceUri;
        BaseUri = baseUri;
    }

    public Uri SourceUri { get; }

    public Uri BaseUri { get; }
}
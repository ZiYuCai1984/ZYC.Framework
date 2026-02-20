using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.MarkdownViewer.Abstractions;

namespace ZYC.Framework.Modules.MarkdownViewer;

[RegisterSingleInstance]
internal class MarkdownViewerTabItemFactory : ITabItemFactory
{
    public bool IsSingle => false;

    public int Priority => 0;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        var uri = context.Uri;

        MarkdownSource? markdownSource = null;

        if (MarkdownRoute.TryParseStrict(uri, out var markdownUri))
        {
            var baseUri = new Uri(markdownUri, ".");
            markdownSource = new MarkdownSource(markdownUri, baseUri);
        }

        return context.Resolve<MarkdownViewerTabItem>(
            new TypedParameter(typeof(MarkdownSource), markdownSource),
            new TypedParameter(typeof(MutableTabReference), new MutableTabReference(context.Uri)));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;

        if (uri.Host == MarkdownViewerModuleConstants.Host)
        {
            return true;
        }

        return false;
    }
}
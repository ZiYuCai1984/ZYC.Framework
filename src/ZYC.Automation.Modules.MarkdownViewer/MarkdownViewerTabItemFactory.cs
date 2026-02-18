using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.MarkdownViewer.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.MarkdownViewer;

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
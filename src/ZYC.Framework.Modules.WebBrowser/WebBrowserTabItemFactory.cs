using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser;

[RegisterSingleInstance]
internal class WebBrowserTabItemFactory : ITabItemFactory
{
    public int Priority => 0;

    public WebBrowserTabItemFactory(IWebBrowserUriPolicy webBrowserUriPolicy)
    {
        WebBrowserUriPolicy = webBrowserUriPolicy;
    }

    private IWebBrowserUriPolicy WebBrowserUriPolicy { get; }

    public bool IsSingle => false;

    public async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<WebBrowserTabItem>(
            new TypedParameter(typeof(MutableTabReference), new MutableTabReference(context.Uri)));
    }

    public async Task<bool> CheckUriMatchedAsync(Uri uri)
    {
        await Task.CompletedTask;
        return WebBrowserUriPolicy.IsAllowed(uri);
    }
}
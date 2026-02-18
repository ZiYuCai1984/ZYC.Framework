using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.WebBrowser.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.WebBrowser;

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
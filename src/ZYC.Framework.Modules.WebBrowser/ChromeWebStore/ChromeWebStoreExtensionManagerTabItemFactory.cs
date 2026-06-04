using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore;

[RegisterSingleInstance]
[TabItemRoute(
    Host = WebBrowserModuleConstants.ChromeWebStoreExtensionManagerHost,
    Path = WebBrowserModuleConstants.ChromeWebStoreExtensionManagerPath)]
internal class ChromeWebStoreExtensionManagerTabItemFactory : TabItemFactoryBase
{
    public override Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        return Task.FromResult<ITabItemInstance>(context.Resolve<ChromeWebStoreExtensionManagerTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri))));
    }
}

using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.WebBrowser.Abstractions;
using ZYC.Framework.Modules.WebBrowser.ChromeWebStore.UI;

namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore;

[Register]
internal class ChromeWebStoreExtensionManagerTabItem :
    TabItemInstanceBase<ChromeWebStoreExtensionManagerView>
{
    public ChromeWebStoreExtensionManagerTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }

    public override string Host => WebBrowserModuleConstants.ChromeWebStoreExtensionManagerHost;

    public override string Title => WebBrowserModuleConstants.ChromeWebStoreExtensionManagerTitle;

    public override string Icon => WebBrowserModuleConstants.ChromeWebStoreExtensionManagerIcon;
}

using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.ChromeExtensions;

[RegisterSingleInstance]
[TabItemRoute(
    Host = ChromeExtensionsModuleConstants.Host)]
internal class ChromeExtensionManagerTabItemFactory : TabItemFactoryBase
{
    public override Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        return Task.FromResult<ITabItemInstance>(context.Resolve<ChromeExtensionManagerTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri))));
    }
}

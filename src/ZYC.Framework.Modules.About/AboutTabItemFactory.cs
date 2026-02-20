using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.About.Abstractions;

namespace ZYC.Framework.Modules.About;

[RegisterSingleInstance]
[TabItemRoute(Host = AboutModuleConstants.Host)]
internal class AboutTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<AboutTabItem>(new TypedParameter(
            typeof(TabReference), new TabReference(context.Uri)));
    }
}
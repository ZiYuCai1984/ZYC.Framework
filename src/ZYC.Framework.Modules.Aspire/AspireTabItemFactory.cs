using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Aspire.Abstractions;

namespace ZYC.Framework.Modules.Aspire;

[RegisterSingleInstance]
[TabItemRoute(Host = AspireModuleContansts.Host)]
internal class AspireTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<AspireTabItem>(new TypedParameter(
            typeof(TabReference), new TabReference(context.Uri)));
    }
}
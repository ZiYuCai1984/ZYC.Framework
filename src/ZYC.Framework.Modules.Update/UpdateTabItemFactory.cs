using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update;

[RegisterSingleInstance]
[TabItemRoute(Host = UpdateModuleConstants.Host)]
internal class UpdateTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<UpdateTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
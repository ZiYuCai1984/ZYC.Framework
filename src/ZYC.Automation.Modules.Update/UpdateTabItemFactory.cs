using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Update.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Update;

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
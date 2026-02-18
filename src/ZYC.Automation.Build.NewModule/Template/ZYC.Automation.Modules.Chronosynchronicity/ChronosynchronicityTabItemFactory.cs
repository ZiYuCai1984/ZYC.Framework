using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Chronosynchronicity.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

// ReSharper disable once CheckNamespace
namespace ZYC.Automation.Modules.Chronosynchronicity;

[RegisterSingleInstance]
[TabItemRoute(Host = ChronosynchronicityModuleConstants.Host)]
internal class ChronosynchronicityTabItemFactory : TabItemFactoryBase
{
    public bool IsSingle => true;

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<ChronosynchronicityTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
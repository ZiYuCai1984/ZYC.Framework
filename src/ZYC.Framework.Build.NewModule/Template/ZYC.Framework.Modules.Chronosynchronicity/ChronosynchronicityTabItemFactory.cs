using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Chronosynchronicity.Abstractions;

// ReSharper disable once CheckNamespace
namespace ZYC.Framework.Modules.Chronosynchronicity;

[RegisterSingleInstance]
[TabItemRoute(Host = ChronosynchronicityModuleConstants.Host)]
internal class ChronosynchronicityTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<ChronosynchronicityTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
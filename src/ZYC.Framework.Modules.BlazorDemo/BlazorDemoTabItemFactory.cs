using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.BlazorDemo.Abstractions;

namespace ZYC.Framework.Modules.BlazorDemo;

[RegisterSingleInstance]
[TabItemRoute(Host = BlazorDemoModuleConstants.Host)]
internal class BlazorDemoTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<BlazorDemoTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
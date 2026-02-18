using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.BlazorDemo.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.BlazorDemo;

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
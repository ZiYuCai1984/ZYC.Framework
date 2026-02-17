using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.About.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.About;

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
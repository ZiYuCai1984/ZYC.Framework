using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Secrets.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Secrets;

[RegisterSingleInstance]
[TabItemRoute(Host = SecretsModuleConstants.Host)]
internal class SecretsTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<SecretsTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
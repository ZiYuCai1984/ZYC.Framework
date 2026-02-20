using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Secrets.Abstractions;

namespace ZYC.Framework.Modules.Secrets;

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
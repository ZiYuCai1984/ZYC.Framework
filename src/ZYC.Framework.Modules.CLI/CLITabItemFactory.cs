using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.CLI.Abstractions;

namespace ZYC.Framework.Modules.CLI;

[RegisterSingleInstance]
[TabItemRoute(Host = CLIModuleConstants.Host)]
internal class CLITabItemFactory : TabItemFactoryBase
{
    public override bool IsSingle => false;

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<CLITabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
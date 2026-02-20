using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager;

[RegisterSingleInstance]
[TabItemRoute(Host = ModuleManagerModuleConstants.Host, Path = ModuleManagerModuleConstants.Local.Path)]
internal class LocalModuleManagerTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<LocalModuleTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
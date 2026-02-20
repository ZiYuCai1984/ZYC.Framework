using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Settings;

[RegisterSingleInstance]
[TabItemRoute(Host = SettingsModuleConstants.Host)]
internal class SettingsTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<SettingsTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
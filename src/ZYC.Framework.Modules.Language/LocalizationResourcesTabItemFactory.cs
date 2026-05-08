using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Language.Abstractions;

namespace ZYC.Framework.Modules.Language;

[RegisterSingleInstance]
[TabItemRoute(Host = LanguageModuleConstants.LocalizationResources.Host, Path = LanguageModuleConstants.LocalizationResources.Path)]
internal class LocalizationResourcesTabItemFactory : TabItemFactoryBase
{
    public override int Priority => 1;

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<LocalizationResourcesTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}

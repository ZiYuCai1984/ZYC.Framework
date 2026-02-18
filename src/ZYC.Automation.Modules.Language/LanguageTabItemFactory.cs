using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Language.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Language;

[RegisterSingleInstance]
[TabItemRoute(Host = LanguageModuleConstants.Host)]
internal class LanguageTabItemFactory : TabItemFactoryBase
{

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;

        return context.Resolve<LanguageTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
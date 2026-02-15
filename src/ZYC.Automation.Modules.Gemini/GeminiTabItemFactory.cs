using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.Gemini.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Modules.Gemini;

[RegisterSingleInstance]
[TabItemRoute(Host = GeminiModuleConstants.Host)]
internal class GeminiTabItemFactory : TabItemFactoryBase
{
    public bool IsSingle => true;

    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<GeminiTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
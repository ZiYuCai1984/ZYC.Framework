using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.__PROJECT_NAME__.Abstractions;

namespace ZYC.Framework.Modules.__PROJECT_NAME__;

[RegisterSingleInstance]
[TabItemRoute(Host = __PROJECT_SHORT_NAME__ModuleConstants.Host)]
internal class __PROJECT_SHORT_NAME__TabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<__PROJECT_SHORT_NAME__TabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}

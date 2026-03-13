using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ApiReference.Abstractions;


namespace ZYC.Framework.Modules.ApiReference;

[RegisterSingleInstance]
[TabItemRoute(Host = ApiReferenceModuleConstants.Host)]
internal class ApiReferenceTabItemFactory : TabItemFactoryBase
{
    public override async Task<ITabItemInstance> CreateTabItemInstanceAsync(TabItemCreationContext context)
    {
        await Task.CompletedTask;
        return context.Resolve<ApiReferenceTabItem>(
            new TypedParameter(typeof(TabReference), new TabReference(context.Uri)));
    }
}
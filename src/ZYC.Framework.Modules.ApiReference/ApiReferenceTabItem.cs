using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.ApiReference.Abstractions;
using ZYC.Framework.Modules.ApiReference.UI;


namespace ZYC.Framework.Modules.ApiReference;

[Register]
[ConstantsSource(typeof(ApiReferenceModuleConstants))]
internal class ApiReferenceTabItem : TabItemInstanceBase<ApiReferenceView>
{
    public ApiReferenceTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}
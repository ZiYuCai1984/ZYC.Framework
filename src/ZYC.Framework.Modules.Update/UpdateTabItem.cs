using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.UI;

namespace ZYC.Framework.Modules.Update;

[Register]
[ConstantsSource(typeof(UpdateModuleConstants))]
internal class UpdateTabItem : TabItemInstanceBase<UpdateView>
{
    public UpdateTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) 
        : base(lifetimeScope, tabReference)
    {
    }
}
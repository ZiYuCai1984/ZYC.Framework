using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Aspire.Abstractions;
using ZYC.Framework.Modules.Aspire.UI;

namespace ZYC.Framework.Modules.Aspire;

[Register]
[ConstantsSource(typeof(AspireModuleContansts))]
internal class AspireTabItem : TabItemInstanceBase<AspireView>
{
    public AspireTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }

    public override bool Localization => false;
}
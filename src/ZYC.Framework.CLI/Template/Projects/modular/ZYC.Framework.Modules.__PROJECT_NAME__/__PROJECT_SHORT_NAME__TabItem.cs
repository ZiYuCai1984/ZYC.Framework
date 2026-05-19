using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.__PROJECT_NAME__.Abstractions;
using ZYC.Framework.Modules.__PROJECT_NAME__.UI;

namespace ZYC.Framework.Modules.__PROJECT_NAME__;

[Register]
[ConstantsSource(typeof(__PROJECT_SHORT_NAME__ModuleConstants))]
internal class __PROJECT_SHORT_NAME__TabItem : TabItemInstanceBase<__PROJECT_SHORT_NAME__View>
{
    public __PROJECT_SHORT_NAME__TabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}

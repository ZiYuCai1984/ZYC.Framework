using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.About.Abstractions;
using ZYC.Framework.Modules.About.UI;

namespace ZYC.Framework.Modules.About;

[Register]
[ConstantsSource(typeof(AboutModuleConstants))]
internal class AboutTabItem : TabItemInstanceBase<AboutView>
{
    public AboutTabItem(ILifetimeScope lifetimeScope, TabReference tabReference)
        : base(lifetimeScope, tabReference)
    {
    }
}
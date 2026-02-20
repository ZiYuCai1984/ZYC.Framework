using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.BlazorDemo.Abstractions;
using ZYC.Framework.Modules.BlazorDemo.UI;

namespace ZYC.Framework.Modules.BlazorDemo;

[Register]
[ConstantsSource(typeof(BlazorDemoModuleConstants))]
internal class BlazorDemoTabItem : TabItemInstanceBase<BlazorDemoView>
{
    public BlazorDemoTabItem(
        ILifetimeScope lifetimeScope, 
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }

    public override bool Localization => false;
}
using Autofac;
using ZYC.Automation.Abstractions.Tab;
using ZYC.Automation.Core.Tab;
using ZYC.Automation.Modules.Chronosynchronicity.Abstractions;
using ZYC.Automation.Modules.Chronosynchronicity.UI;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

// ReSharper disable once CheckNamespace
namespace ZYC.Automation.Modules.Chronosynchronicity;

[Register]
[ConstantsSource(typeof(ChronosynchronicityModuleConstants))]
internal class ChronosynchronicityTabItem : TabItemInstanceBase<ChronosynchronicityView>
{
    public ChronosynchronicityTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}
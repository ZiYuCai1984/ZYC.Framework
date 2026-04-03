using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Chronosynchronicity.Abstractions;
using ZYC.Framework.Modules.Chronosynchronicity.UI;

// ReSharper disable once CheckNamespace
namespace ZYC.Framework.Modules.Chronosynchronicity;

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
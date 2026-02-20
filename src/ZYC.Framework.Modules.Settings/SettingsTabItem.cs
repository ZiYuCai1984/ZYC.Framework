using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Settings.Abstractions;
using ZYC.Framework.Modules.Settings.UI;

namespace ZYC.Framework.Modules.Settings;

[Register]
[ConstantsSource(typeof(SettingsModuleConstants))]
internal class SettingsTabItem : TabItemInstanceBase<SettingsView>
{
    public SettingsTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}
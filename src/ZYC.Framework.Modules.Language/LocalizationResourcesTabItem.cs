using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Language.UI;

namespace ZYC.Framework.Modules.Language;

[Register]
[ConstantsSource(typeof(LanguageModuleConstants.LocalizationResources))]
internal class LocalizationResourcesTabItem : TabItemInstanceBase<LocalizationResourcesView>
{
    public LocalizationResourcesTabItem(ILifetimeScope lifetimeScope, TabReference tabReference)
        : base(lifetimeScope, tabReference)
    {
    }
}

using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;
using ZYC.Framework.Modules.ChromeExtensions.UI;

namespace ZYC.Framework.Modules.ChromeExtensions;

[Register]
[ConstantsSource(typeof(ChromeExtensionsModuleConstants))]
internal class ChromeExtensionManagerTabItem :
    TabItemInstanceBase<ChromeExtensionManagerView>
{
    public ChromeExtensionManagerTabItem(
        ILifetimeScope lifetimeScope,
        TabReference tabReference) : base(lifetimeScope, tabReference)
    {
    }
}
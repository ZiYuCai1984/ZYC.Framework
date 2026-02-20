using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Tab;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.Framework.Modules.ModuleManager.UI;

namespace ZYC.Framework.Modules.ModuleManager;

[Register]
[ConstantsSource(typeof(ModuleManagerModuleConstants.NuGet))]
internal class NuGetModuleTabItem : TabItemInstanceBase<NuGetModuleManagerView>
{
    public NuGetModuleTabItem(ILifetimeScope lifetimeScope, TabReference tabReference) 
        : base(lifetimeScope, tabReference)
    {
    }
}
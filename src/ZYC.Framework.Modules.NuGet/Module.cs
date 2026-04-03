using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.NuGet.Abstractions;

namespace ZYC.Framework.Modules.NuGet;

internal class Module : ModuleBase
{
    public override string Icon => NuGetModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterFileMainMenuItem<NuGetCacheMainMenuItem>();
        return base.LoadAsync(lifetimeScope);
    }
}
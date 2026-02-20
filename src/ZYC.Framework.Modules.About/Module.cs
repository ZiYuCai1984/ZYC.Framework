using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.About.Abstractions;

namespace ZYC.Framework.Modules.About;

internal class Module : ModuleBase
{
    public override string Icon => AboutModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<AboutTabItemFactory>();
        lifetimeScope.RegisterAboutMainMenuItem<AboutMainMenuItem>();

        return Task.CompletedTask;
    }
}
using Autofac;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.About.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.About;

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
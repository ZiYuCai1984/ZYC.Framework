using Autofac;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.ModuleManager.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.MdXaml.Plugins;

namespace ZYC.Automation.Modules.ModuleManager;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        _ = new Definition();

        lifetimeScope.RegisterTabItemFactory<LocalModuleManagerTabItemFactory>();
        lifetimeScope.RegisterTabItemFactory<NuGetModuleManagerTabItemFactory>();

        lifetimeScope.RegisterToolsMainMenuItem<IModuleManagerMainMenuItemsProvider>();

        return Task.CompletedTask;
    }

    public override string Icon => ModuleManagerModuleConstants.Local.Icon;
}
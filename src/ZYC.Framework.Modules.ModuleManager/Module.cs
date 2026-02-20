using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.MdXaml.Plugins;

namespace ZYC.Framework.Modules.ModuleManager;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        _ = new Definition();

        lifetimeScope.RegisterTabItemFactory<LocalModuleManagerTabItemFactory>();
        lifetimeScope.RegisterTabItemFactory<NuGetModuleManagerTabItemFactory>();

        lifetimeScope.RegisterExtensionsMainMenuItem<IModuleManagerMainMenuItemsProvider>();

        return Task.CompletedTask;
    }

    public override string Icon => ModuleManagerModuleConstants.Local.Icon;
}
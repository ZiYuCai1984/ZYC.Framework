using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Settings;

internal class Module : ModuleBase
{
    public override string Icon => SettingsModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<SettingsTabItemFactory>();
        lifetimeScope.RegisterRootMainMenuItem<ISettingsMainMenuItemsProvider>();

        return Task.CompletedTask;
    }
}
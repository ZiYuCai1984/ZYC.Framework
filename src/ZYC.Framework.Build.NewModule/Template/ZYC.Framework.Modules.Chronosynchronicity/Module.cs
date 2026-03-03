using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.Chronosynchronicity.Abstractions;

// ReSharper disable once CheckNamespace
namespace ZYC.Framework.Modules.Chronosynchronicity;

internal class Module : ModuleBase
{
    public override string Icon => ChronosynchronicityModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<ChronosynchronicityTabItemFactory>();
        lifetimeScope.Resolve<IExtensionsMainMenuItemsProvider>()
            .RegisterSubItem<ChronosynchronicityMainMenuItem>();

        return Task.CompletedTask;
    }
}
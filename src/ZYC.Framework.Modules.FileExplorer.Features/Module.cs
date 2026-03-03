using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.FileExplorer.Abstractions;
using ZYC.Framework.Modules.FileExplorer.Features.Abstractions;

namespace ZYC.Framework.Modules.FileExplorer.Features;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.Resolve<IFileMainMenuItemsProvider>()
            .RegisterSubItem<IRecentPathMainMenuItemsProvider>();

        return Task.CompletedTask;
    }

    public override string Icon => FileExplorerFeaturesModuleConstants.Icon;
}
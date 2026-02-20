using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.FileExplorer.Abstractions;

namespace ZYC.Framework.Modules.FileExplorer;

internal class Module : ModuleBase
{
    public override string Icon => FileExplorerModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<FileExplorerTabItemFactory>();
        lifetimeScope.Resolve<IFileMainMenuItemsProvider>()
            .RegisterSubItem<FileExplorerMainMenuItem>();

        return Task.CompletedTask;
    }
}
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.Automation.Modules.FileExplorer.Abstractions;
using ZYC.CoreToolkit.Extensions.Autofac;

namespace ZYC.Automation.Modules.FileExplorer;

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
using Autofac;
using ZYC.Automation.Abstractions.MainMenu;
using ZYC.Automation.Core;
using ZYC.CoreToolkit.Extensions.Autofac;


namespace ZYC.Automation.Modules.MarkdownViewer;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<MarkdownViewerTabItemFactory>();
        lifetimeScope.Resolve<IFileMainMenuItemsProvider>()
            .RegisterSubItem<MarkdownViewerMainMenuItem>();

        return Task.CompletedTask;
    }

    public override string Icon => "📄";
}
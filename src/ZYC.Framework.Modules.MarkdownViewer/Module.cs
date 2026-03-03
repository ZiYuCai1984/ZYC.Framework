using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.MarkdownViewer;

internal class Module : ModuleBase
{
    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<MarkdownViewerTabItemFactory>();
        lifetimeScope.Resolve<IToolsMainMenuItemsProvider>()
            .RegisterSubItem<MarkdownViewerMainMenuItem>();

        return Task.CompletedTask;
    }

    public override string Icon => "📄";
}
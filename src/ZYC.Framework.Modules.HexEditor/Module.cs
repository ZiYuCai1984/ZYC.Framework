using Autofac;
using WpfHexEditor.ProgressBar.Controls;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.HexEditor.Abstractions;

namespace ZYC.Framework.Modules.HexEditor;

internal class Module : ModuleBase
{
    public override string Icon => HexEditorModuleConstants.Icon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        _ = new LinearProgressBar();

        lifetimeScope.RegisterTabItemFactory<HexEditorTabItemFactory>();
        lifetimeScope.Resolve<IFileOpenMainMenuItemsProvider>()
            .RegisterSubItem<HexEditorMainMenuItem>();

        return Task.CompletedTask;
    }
}
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TextEditor.Abstractions;


namespace ZYC.Framework.Modules.TextEditor;

internal class Module : ModuleBase
{
    public override string Icon => TextEditorModuleConstants.PreviewIcon;

    public override Task LoadAsync(ILifetimeScope lifetimeScope)
    {
        lifetimeScope.RegisterTabItemFactory<TextPreviewTabItemFactory>();
        lifetimeScope.RegisterTabItemFactory<TextEditorTabItemFactory>();
        lifetimeScope.Resolve<IFileOpenMainMenuItemsProvider>()
            .RegisterSubItem<TextEditorMainMenuItem>();

        return Task.CompletedTask;
    }
}

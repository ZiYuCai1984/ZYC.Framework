using System.Windows;
using Autofac;
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
        _ = new WpfHexEditor.HexEditor.Converters.ActionToBrushConverter();
        _ = new WpfHexEditor.ProgressBar.Converters.ValueToProgressConverter();
        _ = new WpfHexEditor.ColorPicker.Converters.ColorToBrushConverter();
        _ = new WpfHexEditor.HexBox.Converters.BoolInverterConverter();


        // Search dialogs are separate windows, so editor-local resources are not visible to them.
        var dialogStylesUri = new Uri(
            "pack://application:,,,/ZYC.Framework.Modules.HexEditor;component/UI/HexEditorDialogStyles.xaml");
        var dictionaries = Application.Current.Resources.MergedDictionaries;
        if (dictionaries.All(dictionary => dictionary.Source != dialogStylesUri))
        {
            dictionaries.Add(new ResourceDictionary { Source = dialogStylesUri });
        }

        lifetimeScope.RegisterTabItemFactory<HexEditorTabItemFactory>();
        lifetimeScope.Resolve<IFileOpenMainMenuItemsProvider>()
            .RegisterSubItem<HexEditorMainMenuItem>();

        return Task.CompletedTask;
    }
}

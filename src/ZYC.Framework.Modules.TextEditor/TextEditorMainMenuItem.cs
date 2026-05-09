using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.TextEditor.Abstractions;
using ZYC.Framework.Modules.TextEditor.Commands;

namespace ZYC.Framework.Modules.TextEditor;

[RegisterSingleInstance]
internal class TextEditorMainMenuItem : MainMenuItem
{
    public TextEditorMainMenuItem(
        SelectFileCommand selectFileCommand)
    {
        Info = new MenuItemInfo
        {
            Title = TextEditorModuleConstants.MenuTitle,
            Icon = TextEditorModuleConstants.PreviewIcon
        };

        Command = selectFileCommand;
    }
}
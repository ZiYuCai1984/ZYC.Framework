using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Modules.HexEditor.Abstractions;
using ZYC.Framework.Modules.HexEditor.Commands;

namespace ZYC.Framework.Modules.HexEditor;

[RegisterSingleInstance]
internal class HexEditorMainMenuItem : MainMenuItem
{
    public HexEditorMainMenuItem(SelectFileCommand selectFileCommand)
    {
        Info = new MenuItemInfo
        {
            Title = HexEditorModuleConstants.MenuTitle,
            Icon = HexEditorModuleConstants.Icon
        };

        Command = selectFileCommand;
    }
}

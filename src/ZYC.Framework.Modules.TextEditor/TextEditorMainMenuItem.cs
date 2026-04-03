using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TextEditor.Abstractions;

namespace ZYC.Framework.Modules.TextEditor;

[RegisterSingleInstance]
internal class TextEditorMainMenuItem : MainMenuItem
{
    public TextEditorMainMenuItem(
        ITabManager tabManager,
        IToastManager toastManager)
    {
        Info = new MenuItemInfo
        {
            Title = TextEditorModuleConstants.MenuTitle,
            Icon = TextEditorModuleConstants.Icon
        };

        Command = new RelayCommand(_ => true, async _ =>
        {
            try
            {
                var filePath = DialogTools.SelectFileDialog(filter: TextEditorModuleConstants.FileDialogFilter);
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return;
                }

                await tabManager.NavigateAsync(new Uri(filePath));
            }
            catch (Exception ex)
            {
                toastManager.PromptMessage(ToastMessage.Exception(ex));
            }
        });
    }
}

using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TextEditor.Abstractions;

namespace ZYC.Framework.Modules.TextEditor.Commands;

[RegisterSingleInstance]
internal class SelectFileCommand : AsyncCommandBase
{
    public SelectFileCommand(
        ILogger<SelectFileCommand> logger,
        ITabManager tabManager,
        IToastManager toastManager)
    {
        Logger = logger;
        TabManager = tabManager;
        ToastManager = toastManager;
    }

    private ILogger<SelectFileCommand> Logger { get; }

    private ITabManager TabManager { get; }

    private IToastManager ToastManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        try
        {
            var filePath = DialogTools.SelectFileDialog(filter: TextEditorModuleConstants.FileDialogFilter);
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            await TabManager.NavigateAsync(new Uri(filePath));
        }
        catch (Exception ex)
        {
            ToastManager.PromptException(ex);
            Logger.Error(ex);
        }
    }
}
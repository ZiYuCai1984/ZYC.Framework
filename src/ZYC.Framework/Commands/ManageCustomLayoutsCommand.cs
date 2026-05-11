using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Workspace.CustomLayouts;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class ManageCustomLayoutsCommand : AsyncCommandBase
{
    public ManageCustomLayoutsCommand(
        ILogger<ManageCustomLayoutsCommand> logger,
        IDialogManager dialogManager,
        IToastManager toastManager)
    {
        Logger = logger;
        DialogManager = dialogManager;
        ToastManager = toastManager;
    }

    private ILogger<ManageCustomLayoutsCommand> Logger { get; }

    private IDialogManager DialogManager { get; }

    private IToastManager ToastManager { get; }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        try
        {
            DialogManager.Show<ManageCustomLayoutsDialog>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }

        return Task.CompletedTask;
    }
}

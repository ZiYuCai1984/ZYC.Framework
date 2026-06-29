using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Dialog;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class ShowOverrideMutexIdDialogCommand : AsyncCommandBase
{
    public ShowOverrideMutexIdDialogCommand(
        ILogger<ShowOverrideMutexIdDialogCommand> logger,
        IDialogManager dialogManager,
        IToastManager toastManager)
    {
        Logger = logger;
        DialogManager = dialogManager;
        ToastManager = toastManager;
    }

    private ILogger<ShowOverrideMutexIdDialogCommand> Logger { get; }

    private IDialogManager DialogManager { get; }

    private IToastManager ToastManager { get; }

    protected override Task InternalExecuteAsync(object? parameter)
    {
        try
        {
            DialogManager.Show<OverrideMutexIdDialog>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }

        return Task.CompletedTask;
    }
}
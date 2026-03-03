using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Notification.Toast;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class CopyAndNotifyCommand : AsyncCommandBase
{
    public CopyAndNotifyCommand(IToastManager toastManager)
    {
        ToastManager = toastManager;
    }

    private IToastManager ToastManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        await Task.CompletedTask;

        var value = parameter?.ToString();

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        ClipboardTools.SetText(value);
        ToastManager.PromptCopied(value);
    }

    public override bool CanExecute(object? parameter)
    {
        var value = parameter?.ToString();
        return !string.IsNullOrEmpty(value);
    }
}
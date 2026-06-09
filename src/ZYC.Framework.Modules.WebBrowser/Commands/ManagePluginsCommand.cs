using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.WebBrowser.Dialog;

namespace ZYC.Framework.Modules.WebBrowser.Commands;

[RegisterSingleInstance]
internal class ManagePluginsCommand : CommandBase
{
    public ManagePluginsCommand(
        ILogger<ManagePluginsCommand> logger,
        IDialogManager dialogManager,
        IToastManager toastManager)
    {
        Logger = logger;
        DialogManager = dialogManager;
        ToastManager = toastManager;
    }

    private ILogger<ManagePluginsCommand> Logger { get; }

    private IDialogManager DialogManager { get; }

    private IToastManager ToastManager { get; }

    protected override void InternalExecute(object? parameter)
    {
        try
        {
            DialogManager.Show<ManagePluginsDialog>();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }
}
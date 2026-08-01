using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ModuleManager.UI;

namespace ZYC.Framework.Modules.ModuleManager.Commands;

[RegisterSingleInstance]
internal class ShowManualInstallNuGetModuleDialogCommand : CommandBase
{
    public ShowManualInstallNuGetModuleDialogCommand(
        IDialogManager dialogManager,
        IToastManager toastManager,
        IAppLogger<ShowManualInstallNuGetModuleDialogCommand> logger)
    {
        DialogManager = dialogManager;
        ToastManager = toastManager;
        Logger = logger;
    }

    private IDialogManager DialogManager { get; }

    private IToastManager ToastManager { get; }

    private IAppLogger<ShowManualInstallNuGetModuleDialogCommand> Logger { get; }

    protected override void InternalExecute(object? parameter)
    {
        try
        {
            DialogManager.Show<ManualInstallNuGetModuleDialog>();
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            ToastManager.PromptException(exception);
        }
    }
}

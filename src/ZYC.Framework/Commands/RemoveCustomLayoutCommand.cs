using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class RemoveCustomLayoutCommand : AsyncCommandBase<CustomWorkspaceLayout>
{
    public RemoveCustomLayoutCommand(
        ILogger<RemoveCustomLayoutCommand> logger,
        CustomWorkspaceLayoutConfig customWorkspaceLayoutConfig,
        IToastManager toastManager)
    {
        Logger = logger;
        CustomWorkspaceLayoutConfig = customWorkspaceLayoutConfig;
        ToastManager = toastManager;
    }

    private ILogger<RemoveCustomLayoutCommand> Logger { get; }

    private CustomWorkspaceLayoutConfig CustomWorkspaceLayoutConfig { get; }

    private IToastManager ToastManager { get; }

    protected override async Task InternalExecuteAsync(CustomWorkspaceLayout parameter)
    {
        await Task.CompletedTask;

        try
        {
            var layouts = CustomWorkspaceLayoutConfig.Layouts
                .Where(t => t.Id != parameter.Id)
                .ToArray();

            CustomWorkspaceLayoutConfig.Layouts = layouts;

            ToastManager.PromptMessage(ToastMessage.Info("Custom layout removed."));
        }
        catch (Exception e)
        {
            Logger.Error(e);
            ToastManager.PromptException(e);
        }
    }

    protected override bool InternalCanExecute(CustomWorkspaceLayout? parameter)
    {
        return parameter != null;
    }
}

using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.MainMenu;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.MainMenu.BuildIn;

[Register]
internal class ApplyCustomLayoutMainMenuItem : MainMenuItem
{
    public ApplyCustomLayoutMainMenuItem(
        ILogger<ApplyCustomLayoutMainMenuItem> logger,
        IParallelWorkspaceManager parallelWorkspaceManager,
        IToastManager toastManager,
        CustomWorkspaceLayout customWorkspaceLayout)
    {
        Logger = logger;
        ParallelWorkspaceManager = parallelWorkspaceManager;
        ToastManager = toastManager;
        CustomWorkspaceLayout = customWorkspaceLayout;

        Info = new MenuItemInfo
        {
            Title = customWorkspaceLayout.Name,
            Icon = customWorkspaceLayout.Thumbnail,
            Localization = false
        };

        // ReSharper disable once AsyncVoidLambda
        Command = new RelayCommand(_ => true, async _ =>
        {
            try
            {
                await ParallelWorkspaceManager.ApplyLayoutAsync(CustomWorkspaceLayout.WorkspaceNode);

                ToastManager.PromptMessage(ToastMessage.Info("Custom layout applied."));
            }
            catch (Exception e)
            {
                Logger.Error(e);
                ToastManager.PromptException(e);
            }
        });
    }

    private ILogger<ApplyCustomLayoutMainMenuItem> Logger { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    private IToastManager ToastManager { get; }

    private CustomWorkspaceLayout CustomWorkspaceLayout { get; }
}

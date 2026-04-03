using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class SaveCustomLayoutCommand : AsyncCommandBase
{
    public SaveCustomLayoutCommand(
        ILogger<SaveCustomLayoutCommand> logger,
        CustomWorkspaceLayoutConfig customWorkspaceLayoutconfig,
        RootWorkspaceNodeState rootWorkspaceNodeState,
        IAppContext appContext,
        IToastManager toastManager)
    {
        Logger = logger;
        CustomWorkspaceLayoutConfig = customWorkspaceLayoutconfig;
        RootWorkspaceNodeState = rootWorkspaceNodeState;
        AppContext = appContext;
        ToastManager = toastManager;
    }

    private ILogger<SaveCustomLayoutCommand> Logger { get; }

    private CustomWorkspaceLayoutConfig CustomWorkspaceLayoutConfig { get; }

    private RootWorkspaceNodeState RootWorkspaceNodeState { get; }

    private IAppContext AppContext { get; }

    private IToastManager ToastManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        await Task.CompletedTask;

        try
        {
            var layouts = CustomWorkspaceLayoutConfig.Layout.ToList();

            var node = JsonTools.DeepCopy(RootWorkspaceNodeState);
            node.NavigationState = new NavigationState();

            var id = Guid.NewGuid();

            layouts.Add(new CustomWorkspaceLayout
            {
                Id = id,
                //TODO-zyc CustomWorkspaceLayout Name
                Name = Guid.NewGuid().ToString(),
                WorkspaceNode = node
            });

            CustomWorkspaceLayoutConfig.Layout = layouts.ToArray();

            ToastManager.PromptMessage(
                ToastMessage.Info("Current layout saved to custom layout."));
        }
        catch (Exception e)
        {
            Logger.Error(e);
            ToastManager.PromptException(e);
        }
    }
}
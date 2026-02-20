using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Modules.Mock.Commands;

[RegisterSingleInstance]
internal class MergeAllWorkspaceCommand : CommandBase
{
    public MergeAllWorkspaceCommand(
        IAppContext appContext,
        IParallelWorkspaceManager parallelWorkspaceManager,
        IAppBusyWindow appBusyWindow)
    {
        AppContext = appContext;
        ParallelWorkspaceManager = parallelWorkspaceManager;
        AppBusyWindow = appBusyWindow;
    }

    private IAppContext AppContext { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    private IAppBusyWindow AppBusyWindow { get; }

    protected override void InternalExecute(object? parameter)
    {
        _ = Task.Run(async () =>
        {
            using var handler = AppBusyWindow.Enqueue();
            handler.ShowProgress = true;
            handler.IsProgressIndeterminate = false;

            var count = ParallelWorkspaceManager.GetWorkspaceDictionary().Keys.Count();

            while (true)
            {
                var dic = ParallelWorkspaceManager.GetWorkspaceDictionary();

                handler.ProgressValue = (count - dic.Count + 0.0) / count;
                handler.Title = $"Merging {count - dic.Count}/{count}";

                if (dic.Count == 1)
                {
                    return;
                }

                await AppContext.InvokeOnUIThreadAsync(async () =>
                {
                    var foucs = ParallelWorkspaceManager.GetFocusedWorkspace();
                    await ParallelWorkspaceManager.MergeAsync(foucs);
                });
            }
        });


        base.InternalExecute(parameter);
    }
}
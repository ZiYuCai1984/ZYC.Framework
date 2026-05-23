using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
internal class CloseOtherTabsCommand : AsyncCommandBase<ITabItemInstance>
{
    public CloseOtherTabsCommand(
        ITabManager tabManager,
        TabItemLockState tabItemLockState,
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        TabManager = tabManager;
        TabItemLockState = tabItemLockState;
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private ITabManager TabManager { get; }

    private TabItemLockState TabItemLockState { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    protected override async Task InternalExecuteAsync(ITabItemInstance parameter)
    {
        var workspace = TabManager.GetTabItemInstanceWorkspace(parameter);
        var tabInstances = TabManager.GetTabItemInstances(workspace.Id);

        foreach (var tabInstance in tabInstances)
        {
            if (tabInstance == parameter)
            {
                continue;
            }

            await TabManager.CloseAsync(tabInstance);
        }
    }

    protected override bool InternalCanExecute(ITabItemInstance? parameter)
    {
        if (parameter == null)
        {
            return false;
        }


        var workspace = TabManager.GetTabItemInstanceWorkspace(parameter);
        var tabInstances = TabManager.GetTabItemInstances(workspace.Id);

        foreach (var tabInstance in tabInstances)
        {
            if (tabInstance == parameter)
            {
                continue;
            }

            if (TabItemLockState.TabItems.Contains(tabInstance.TabReference))
            {
                continue;
            }


            return true;
        }

        return false;
    }
}
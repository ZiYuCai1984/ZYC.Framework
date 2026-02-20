using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class SwapCommand : AsyncCommandBase<WorkspaceNode>
{
    public SwapCommand(
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    protected override async Task InternalExecuteAsync(WorkspaceNode parameter)
    {
        await ParallelWorkspaceManager.SwapAsync(parameter);
    }

    protected override bool InternalCanExecute(WorkspaceNode? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        return ParallelWorkspaceManager.CanSwap(parameter);
    }
}
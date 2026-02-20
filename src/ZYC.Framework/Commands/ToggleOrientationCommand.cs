using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class ToggleOrientationCommand : AsyncCommandBase<WorkspaceNode>
{
    public ToggleOrientationCommand(
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    protected override Task InternalExecuteAsync(WorkspaceNode parameter)
    {
        return ParallelWorkspaceManager.ToggleOrientationAsync(parameter);
    }


    protected override bool InternalCanExecute(WorkspaceNode? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        return ParallelWorkspaceManager.CanToggleOrientation(parameter);
    }
}
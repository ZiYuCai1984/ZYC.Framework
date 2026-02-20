using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class SetFocusedWorkspaceCommand : CommandBase<WorkspaceNode>
{
    public SetFocusedWorkspaceCommand(
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    protected override void InternalExecute(WorkspaceNode parameter)
    {
        ParallelWorkspaceManager.SetFocusedWorkspace(parameter);
    }

    protected override bool InternalCanExecute(WorkspaceNode parameter)
    {
        return ParallelWorkspaceManager.GetFocusedWorkspace() != parameter;
    }
}
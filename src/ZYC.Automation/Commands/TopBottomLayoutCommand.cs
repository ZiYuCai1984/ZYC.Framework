using Autofac;
using ZYC.Automation.Abstractions.State;
using ZYC.Automation.Abstractions.Workspace;
using ZYC.Automation.Core.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Commands;

[RegisterSingleInstance]
internal class TopBottomLayoutCommand : AsyncCommandBase
{
    public TopBottomLayoutCommand(
        RootWorkspaceNodeState rootWorkspaceNode,
        ILifetimeScope lifetimeScope,
        IParallelWorkspaceManager parallelWorkspaceManager)
    {
        RootWorkspaceNode = rootWorkspaceNode;
        LifetimeScope = lifetimeScope;
        ParallelWorkspaceManager = parallelWorkspaceManager;
    }

    private RootWorkspaceNodeState RootWorkspaceNode { get; }

    private ILifetimeScope LifetimeScope { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        await ParallelWorkspaceManager.MergeAllAsync();
        await ParallelWorkspaceManager.SplitVerticalAsync(RootWorkspaceNode);
    }
}
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

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
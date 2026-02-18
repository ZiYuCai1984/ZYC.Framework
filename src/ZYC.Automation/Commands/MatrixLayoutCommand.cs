using Autofac;
using ZYC.Automation.Abstractions.State;
using ZYC.Automation.Abstractions.Workspace;
using ZYC.Automation.Core.Commands;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Automation.Commands;

[RegisterSingleInstance]
internal class MatrixLayoutCommand : AsyncCommandBase
{
    public MatrixLayoutCommand(
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
        static bool ShouldSplitHorizontalByIndex(int splitIndex)
        {
            // k = floor(log2(i+1))
            var k = FloorLog2(splitIndex + 1);

            // Even levels: horizontal; odd levels: vertical
            return k % 2 == 0;
        }

        await ParallelWorkspaceManager.MergeAllAsync();


        for (var i = 0; i < 3; ++i)
        {
            var node = RootWorkspaceNode.FindShallowestLeaf()!;
            if (ShouldSplitHorizontalByIndex(i))
            {
                await ParallelWorkspaceManager.SplitHorizontalAsync(node);
            }
            else
            {
                await ParallelWorkspaceManager.SplitVerticalAsync(node);
            }
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="x">x > 0</param>
    /// <returns></returns>
    private static int FloorLog2(int x)
    {
        var k = 0;
        while ((x >>= 1) != 0)
        {
            k++;
        }

        return k;
    }
}
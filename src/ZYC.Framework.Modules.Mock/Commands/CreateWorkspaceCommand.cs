using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Modules.Mock.Commands;

[Register]
internal class CreateWorkspaceCommand : CommandBase<int>
{
    public CreateWorkspaceCommand(
        RootWorkspaceNodeState rootWorkspaceNode,
        CreateWorkspaceOptions createWorkspaceOptions,
        IAppContext appContext,
        IParallelWorkspaceManager parallelWorkspaceManager,
        IAppBusyWindow appBusyWindow)
    {
        RootWorkspaceNode = rootWorkspaceNode;
        CreateWorkspaceOptions = createWorkspaceOptions;
        AppContext = appContext;
        ParallelWorkspaceManager = parallelWorkspaceManager;
        AppBusyWindow = appBusyWindow;
    }

    private IAppContext AppContext { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    private IAppBusyWindow AppBusyWindow { get; }

    private RootWorkspaceNodeState RootWorkspaceNode { get; }

    private CreateWorkspaceOptions CreateWorkspaceOptions { get; }

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


    private void CreateBlancedWorkspace(int num)
    {
        static bool ShouldSplitHorizontalByIndex(int splitIndex)
        {
            // k = floor(log2(i+1))
            var k = FloorLog2(splitIndex + 1);

            // Even levels: horizontal; odd levels: vertical
            return k % 2 == 0;
        }

        _ = Task.Run(async () =>
        {
            using var handler = AppBusyWindow.Enqueue();
            handler.ShowProgress = true;
            handler.IsProgressIndeterminate = false;

            for (var i = 0; i < num; ++i)
            {
                handler.ProgressValue = (i + 1.0) / num;
                handler.Title = $"Creating {i + 1.0}/{num}";

                await AppContext.InvokeOnUIThreadAsync(async () =>
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
                });
            }
        });
    }

    private void CreateRecursiveWorkspace(int num)
    {
        _ = Task.Run(async () =>
        {
            using var handler = AppBusyWindow.Enqueue();
            handler.ShowProgress = true;
            handler.IsProgressIndeterminate = false;

            for (var i = 0; i < num; ++i)
            {
                handler.ProgressValue = (i + 1.0) / num;
                handler.Title = $"Creating {i + 1.0}/{num}";

                await AppContext.InvokeOnUIThreadAsync(async () =>
                {
                    var workspaceNode = ParallelWorkspaceManager.GetFocusedWorkspace();

                    if ((i + 2) % 2 == 0)
                    {
                        await ParallelWorkspaceManager.SplitHorizontalAsync(workspaceNode);
                    }
                    else
                    {
                        await ParallelWorkspaceManager.SplitVerticalAsync(workspaceNode);
                    }
                });
            }
        });
    }

    private void CreateHorizontalVerticalWorkspace(int num, bool isHorizontal)
    {
        _ = Task.Run(async () =>
        {
            using var handler = AppBusyWindow.Enqueue();
            handler.ShowProgress = true;
            handler.IsProgressIndeterminate = false;

            for (var i = 0; i < num; ++i)
            {
                handler.ProgressValue = (i + 1.0) / num;
                handler.Title = $"Creating {i + 1.0}/{num}";

                await AppContext.InvokeOnUIThreadAsync(async () =>
                {
                    var node = RootWorkspaceNode.FindShallowestLeaf()!;
                    if (isHorizontal)
                    {
                        await ParallelWorkspaceManager.SplitHorizontalAsync(node);
                    }
                    else
                    {
                        await ParallelWorkspaceManager.SplitVerticalAsync(node);
                    }
                });
            }
        });
    }


    protected override void InternalExecute(int num)
    {
        switch (CreateWorkspaceOptions)
        {
            case CreateWorkspaceOptions.Balanced:
                CreateBlancedWorkspace(num);
                break;
            case CreateWorkspaceOptions.Recursive:
                CreateRecursiveWorkspace(num);
                break;
            case CreateWorkspaceOptions.Horizontal:
                CreateHorizontalVerticalWorkspace(num, true);
                break;
            case CreateWorkspaceOptions.Vertical:
                CreateHorizontalVerticalWorkspace(num, false);
                break;
            default:
                DebuggerTools.Break();
                throw new InvalidOperationException();
        }
    }
}

internal enum CreateWorkspaceOptions
{
    Balanced,
    Recursive,
    Horizontal,
    Vertical
}
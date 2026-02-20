using System.Collections.Frozen;
using System.Diagnostics;
using System.Windows;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Tab;

namespace ZYC.Framework.Workspace;

[RegisterSingleInstanceAs(
    typeof(ParallelWorkspaceView), typeof(IParallelWorkspaceManager))]
internal partial class ParallelWorkspaceView : IParallelWorkspaceManager
{
    public ParallelWorkspaceView(
        IEventAggregator eventAggregator,
        ParallelWorkspaceState parallelWorkspaceState,
        ILifetimeScope lifetimeScope,
        RootWorkspaceNodeState rootWorkspaceNodeState)
    {
        EventAggregator = eventAggregator;
        ParallelWorkspaceState = parallelWorkspaceState;
        LifetimeScope = lifetimeScope;
        RootWorkspaceNodeState = rootWorkspaceNodeState;

        InitializeComponent();
    }

    private IEventAggregator EventAggregator { get; }

    private ParallelWorkspaceState ParallelWorkspaceState { get; }

    private ILifetimeScope LifetimeScope { get; }

    private RootWorkspaceNodeState RootWorkspaceNodeState { get; }

    private bool FirstRending { get; set; } = true;

    private IDictionary<Guid, WorkspaceView> WorkspaceViewDictionary { get; } = new Dictionary<Guid, WorkspaceView>();

    private IDictionary<Guid, WorkspaceNode> WorkspaceDictionary { get; } = new Dictionary<Guid, WorkspaceNode>();

    private async void OnParallelWorkspaceViewLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!FirstRending)
            {
                return;
            }

            FirstRending = false;

            var root = LifetimeScope.Resolve<WorkspaceView>(
                new TypedParameter(typeof(WorkspaceNode), RootWorkspaceNodeState),
                new TypedParameter(typeof(WorkspaceView), null),
#pragma warning disable CS8974
                new TypedParameter(typeof(Func<WorkspaceView, Task<object>>), CreateTabManagerViewAsync));

            Content = root;

            await TabManager.RestoreStateAsync();

            EventAggregator.Publish(new TabManagerRestoreCompleted());
        }
        catch
        {
            //ignore
        }
    }


    private ITabManager? _tabManager;


    public ITabManager TabManager => _tabManager ??= LifetimeScope.Resolve<ITabManager>();


    public async Task ResetAsync()
    {
        var target = WorkspaceViewDictionary.Values.FirstOrDefault()
                     ?? Content as WorkspaceView;
        if (target == null)
        {
            Debugger.Break();
            return;
        }

        await TabManager.CloseAllAsync();
        await target.ResetAsync();
    }

    public bool CanMerge(WorkspaceNode workspaceNode)
    {
        if (!WorkspaceViewDictionary.ContainsKey(workspaceNode.Id))
        {
            return false;
        }

        var workspaceView = WorkspaceViewDictionary[workspaceNode.Id];
        return !workspaceView.IsRoot;
    }

    public async Task ToggleOrientationAsync(WorkspaceNode workspaceNode)
    {
        var id = workspaceNode.Id;
        if (!WorkspaceViewDictionary.ContainsKey(id))
        {
            Debugger.Break();
            return;
        }

        var workspaceView = WorkspaceViewDictionary[id];
        await workspaceView.ToggleOrientationAsync();

        PublishWorkspaceFocusChangedEvent();
    }

    public bool CanToggleOrientation(WorkspaceNode workspaceNode)
    {
        var id = workspaceNode.Id;
        if (!WorkspaceViewDictionary.ContainsKey(id))
        {
            return false;
        }

        var parentWorkspaceView = WorkspaceViewDictionary[id].ParentWorkspaceView;
        return parentWorkspaceView != null;
    }


    public IReadOnlyDictionary<Guid, WorkspaceNode> GetWorkspaceDictionary()
    {
        return WorkspaceDictionary.ToFrozenDictionary();
    }

    public async Task SwapAsync(WorkspaceNode workspaceNode)
    {
        var id = workspaceNode.Id;
        if (!WorkspaceViewDictionary.ContainsKey(id))
        {
            Debugger.Break();
            return;
        }

        var workspaceView = WorkspaceViewDictionary[id];
        await workspaceView.SwapAsync();

        PublishWorkspaceFocusChangedEvent();
    }

    public bool CanSwap(WorkspaceNode workspaceNode)
    {
        var id = workspaceNode.Id;
        if (!WorkspaceViewDictionary.ContainsKey(id))
        {
            return false;
        }

        var parentWorkspaceView = WorkspaceViewDictionary[id].ParentWorkspaceView;
        return parentWorkspaceView != null;
    }

    public void SetHighlight(WorkspaceNode workspace, bool highlight)
    {
        EventAggregator.Publish(new WorkspaceHighlightEvent(workspace, highlight));
    }

    public async Task MergeAllAsync()
    {
        while (true)
        {
            var dic = GetWorkspaceDictionary();

            if (dic.Count == 1)
            {
                return;
            }

            var foucs = GetFocusedWorkspace();
            await MergeAsync(foucs);
        }
    }

    private async Task<object> CreateTabManagerViewAsync(WorkspaceView workspaceView)
    {
        await Task.CompletedTask;

        var workspace = workspaceView.Node;

        TraceWriteLine("");
        TraceWriteLine($"***Create {workspace.Id}***");

        RootWorkspaceNodeState.Trace();

        TraceWriteLine("*******************************************************");
        TraceWriteLine("");

        AttachWorkspaceView(workspace.Id, workspaceView);
        SetFocusedWorkspace(workspace);


        //!WARNING Considering the life cycle of TabItem objects and their movement in multiple Workspaces, child lifetimeScope is not used here.

        return LifetimeScope.Resolve<TabManagerView>(
            new TypedParameter(typeof(WorkspaceNode), workspace),
            new TypedParameter(typeof(Func<WorkspaceNode, Task>), DisposeTabManagerViewAsync));
    }


    private static void TraceWriteLine(string s)
    {
#if DEBUG
        Trace.WriteLine(s);
#endif
    }

    public async Task DisposeTabManagerViewAsync(WorkspaceNode workspaceNode)
    {
        await Task.CompletedTask;

        TraceWriteLine("");
        TraceWriteLine($"***Dispose {workspaceNode.Id}***");

        RootWorkspaceNodeState.Trace();

        TraceWriteLine("*******************************************************");
        TraceWriteLine("");


        DetachWorkspaceView(workspaceNode.Id);
    }

    private void AttachWorkspaceView(Guid id, WorkspaceView workspaceView)
    {
        if (!WorkspaceDictionary.ContainsKey(id))
        {
            WorkspaceDictionary.Add(id, workspaceView.Node);
            WorkspaceViewDictionary.Add(id, workspaceView);
        }
        else
        {
            Debugger.Break();
        }
    }

    private void DetachWorkspaceView(Guid id)
    {
        if (!WorkspaceDictionary.ContainsKey(id))
        {
            return;
        }

        var focusedWorkspace = FocusedWorkspaceNode;
        var toId = focusedWorkspace.Id;

        WorkspaceNode? fallbackWorkspace;

        if (WorkspaceDictionary.TryGetValue(toId, out var candidate)
            && candidate.Id != id)
        {
            fallbackWorkspace = candidate;
        }
        else
        {
            fallbackWorkspace = WorkspaceDictionary
                .Where(kv => kv.Key != id)
                .Select(kv => kv.Value)
                .FirstOrDefault();
        }

        var targetId = fallbackWorkspace?.Id ?? id;
        TabManager.MoveAllTabItemInstances(id, targetId);

        WorkspaceDictionary.Remove(id);
        WorkspaceViewDictionary.Remove(id);

        if (ParallelWorkspaceState.FocusedWorkspaceId != id)
        {
            return;
        }

        if (fallbackWorkspace != null)
        {
            SetFocusedWorkspace(fallbackWorkspace);
            return;
        }

        ParallelWorkspaceState.FocusedWorkspaceId = Guid.Empty;
        PublishWorkspaceFocusChangedEvent();
    }

    public void SetFocusedWorkspace(WorkspaceNode workspaceNode)
    {
        FocusedWorkspaceNode = workspaceNode;
    }

    public WorkspaceNode GetFocusedWorkspace()
    {
        return FocusedWorkspaceNode;
    }

    public async Task SplitHorizontalAsync(WorkspaceNode workspaceNode)
    {
        await WorkspaceViewDictionary[workspaceNode.Id].SplitAsync(true);
    }

    public async Task SplitVerticalAsync(WorkspaceNode workspaceNode)
    {
        await WorkspaceViewDictionary[workspaceNode.Id].SplitAsync(false);
    }

    public async Task MergeAsync(WorkspaceNode workspaceNode)
    {
        await WorkspaceViewDictionary[workspaceNode.Id].MergeAsync();
    }

    private void PublishWorkspaceFocusChangedEvent(Guid? id = null)
    {
        EventAggregator.Publish(new WorkspaceFocusChangedEvent(id));
    }

    private WorkspaceNode FocusedWorkspaceNode
    {
        get
        {
            var id = ParallelWorkspaceState.FocusedWorkspaceId;
            var target = RootWorkspaceNodeState.FindNodeWithIdAndLeftNull(id)
                         ?? RootWorkspaceNodeState.FindFirstLeftNull()!;

            if (target.Id != id)
            {
                ParallelWorkspaceState.FocusedWorkspaceId = target.Id;
                PublishWorkspaceFocusChangedEvent(target.Id);
            }

            return target;
        }
        set
        {
            ParallelWorkspaceState.FocusedWorkspaceId = value.Id;

            PublishWorkspaceFocusChangedEvent(value.Id);
        }
    }
}
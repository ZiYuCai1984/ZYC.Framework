using System.ComponentModel;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Overlay;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core;

namespace ZYC.Framework.Workspace;

[Register]
internal sealed partial class WorkspaceView : INotifyPropertyChanged
{
    public WorkspaceView(
        IToastManager toastManager,
        IOverlayManager overlayManager,
        IEventAggregator eventAggregator,
        ILifetimeScope lifetimeScope,
        WorkspaceNode node,
        WorkspaceView? parentWorkspaceView,
        Func<WorkspaceView, Task<object>> buildLeafContentFunc)
    {
        eventAggregator.Subscribe<WorkspaceHighlightEvent>(OnWorkspaceHighlightChanged)
            .DisposeWith(CompositeDisposable);

        ToastManager = toastManager;
        OverlayManager = overlayManager;

        Node = node ?? throw new ArgumentNullException(nameof(node));

        ParentWorkspaceView = parentWorkspaceView;
        LifetimeScope = lifetimeScope;
        BuildLeafContentFunc = buildLeafContentFunc;

        //Only check once in root
        if (IsRoot)
        {
            ValidateNode(Node);
        }

        InitializeComponent();

        _ = RenderNodeAsync();
    }

    private IToastManager ToastManager { get; }

    private IOverlayManager OverlayManager { get; }

    public WorkspaceNode Node { get; }

    internal WorkspaceView? ParentWorkspaceView { get; }

    private ILifetimeScope LifetimeScope { get; }

    public bool IsMenuVisible => Node.Left == null;

    public bool IsRoot => ParentWorkspaceView == null;

    private Func<WorkspaceView, Task<object>> BuildLeafContentFunc { get; }

    private IOverlay? Overlay { get; set; }


    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnWorkspaceHighlightChanged(WorkspaceHighlightEvent e)
    {
        if (e.WorkspaceNode != Node)
        {
            return;
        }

        if (e.Highlight)
        {
            Overlay = OverlayManager.Show(this, this);
        }
        else
        {
            Overlay?.Dispose();
        }
    }


    private static void ValidateNode(WorkspaceNode node)
    {
        if (node.Left == null != (node.Right == null))
        {
            throw new InvalidOperationException(
                "The Workspace node is illegal: Left and Right must both exist or be null.");
        }

        if (node.Left != null)
        {
            ValidateNode(node.Left);
        }

        if (node.Right != null)
        {
            ValidateNode(node.Right);
        }
    }

    private async Task RenderNodeAsync()
    {
        try
        {
            var ori = Border.Child;

            if (Node.Left == null)
            {
                Border.Child = await BuildLeafContentAsync();
            }
            else
            {
                Border.Child = BuildSplitContent();
            }

            ori?.TryDispose();

            OnPropertyChanged(nameof(IsMenuVisible));
        }
        catch
        {
            //ignore
        }
    }

    private async Task<UIElement> BuildLeafContentAsync()
    {
        var element = (UIElement)await BuildLeafContentFunc.Invoke(this);
        return element;
    }

    private UIElement BuildSplitContent()
    {
        var split = new SplitView
        {
            Orientation = Node.IsHorizontal ? Orientation.Horizontal : Orientation.Vertical,
            DataContext = Node
        };

        split.SetBinding(SplitView.RatioProperty, nameof(WorkspaceNode.Ratio));


        split.RightContent = ResolveChildNode(Node.Right!);
        split.LeftContent = ResolveChildNode(Node.Left!);

        return split;
    }

    private WorkspaceView ResolveChildNode(WorkspaceNode leftOrRight)
    {
        return LifetimeScope.Resolve<WorkspaceView>(
            new TypedParameter(typeof(WorkspaceNode), leftOrRight),
            new TypedParameter(typeof(WorkspaceView), this),
            new TypedParameter(typeof(Func<WorkspaceView, Task<object>>), BuildLeafContentFunc));
    }


    internal async Task SplitAsync(bool isHorizontal)
    {
        if (Node.Left == null)
        {
            Node.IsHorizontal = isHorizontal;

            Node.Left = CreateWorkspace();
            Node.Right = CreateWorkspace();

            await RenderNodeAsync();
        }
    }

    private WorkspaceNode CreateWorkspace()
    {
        var root = LifetimeScope.Resolve<RootWorkspaceNodeState>();
        var index = root.AllocateNextIndex();

        var id = Guid.NewGuid();
        return new WorkspaceNode { Id = id, Index = index };
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public async Task MergeAsync()
    {
        if (ParentWorkspaceView == null)
        {
            return;
        }

        ParentWorkspaceView.Node.Left = null;
        ParentWorkspaceView.Node.Right = null;

        await ParentWorkspaceView.RenderNodeAsync();
    }

    public async Task ToggleOrientationAsync()
    {
        if (ParentWorkspaceView == null)
        {
            return;
        }

        await ParentWorkspaceView.ImplToggleOrientationAsync();
    }


    private async Task ImplToggleOrientationAsync()
    {
        await Task.CompletedTask;

        Node.IsHorizontal = !Node.IsHorizontal;

        var splitView = (SplitView)Border.Child;
        splitView.Orientation = Node.IsHorizontal ? Orientation.Horizontal : Orientation.Vertical;
    }


    public async Task SwapAsync()
    {
        if (ParentWorkspaceView == null)
        {
            return;
        }

        await ParentWorkspaceView.ImplSwapAsync();
    }


    private async Task ImplSwapAsync()
    {
        await Task.CompletedTask;
        var splitView = (SplitView)Border.Child;

        var left = splitView.LeftContent;
        splitView.LeftContent = null;

        var right = splitView.RightContent;
        splitView.RightContent = null;

        splitView.RightContent = left;
        splitView.LeftContent = right;
        splitView.Ratio = 1 - splitView.Ratio;
    }

    public async Task ResetAsync()
    {
        if (!IsRoot)
        {
            var root = FindRootWorkspaceView(this);
            if (!ReferenceEquals(root, this))
            {
                await root.ResetAsync();
            }

            return;
        }

        Border.Child?.TryDispose();
        Border.Child = null;

        ResetWorkspace(Node);

        await RenderNodeAsync();
    }


    private static void ResetWorkspace(WorkspaceNode workspaceNode)
    {
        workspaceNode.Left = null;
        workspaceNode.Right = null;
        workspaceNode.Ratio = 0.5;
        workspaceNode.IsHorizontal = true;
        workspaceNode.NavigationState = new NavigationState();
        workspaceNode.Id = Guid.NewGuid();
    }

    private WorkspaceView FindRootWorkspaceView(WorkspaceView view)
    {
        if (view.ParentWorkspaceView == null)
        {
            return view;
        }

        return FindRootWorkspaceView(view.ParentWorkspaceView);
    }
}
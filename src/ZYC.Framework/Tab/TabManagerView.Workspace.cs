using System.Windows.Input;
using Autofac;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Tab;

internal partial class TabManagerView
{
    private IParallelWorkspaceManager? _parallelWorkspaceManager;

    private IWorkspaceContextMenuManager? _workspaceContextMenuManager;

    public bool IsFocusedWorkspace
    {
        get
        {
            var workspace = ParallelWorkspaceManager.GetFocusedWorkspace();
            return workspace == WorkspaceNode;
        }
    }

    public WorkspaceNode WorkspaceNode { get; }

    public IWorkspaceMenuItem[] WorkspaceContextMenuItems => WorkspaceContextMenuManager.GetItems();

    private WorkspaceConfig WorkspaceConfig { get; }

    public bool IsWorkspaceEmptyIndexVisible
    {
        get => WorkspaceConfig.IsWorkspaceEmptyIndexVisible;
        set => WorkspaceConfig.IsWorkspaceEmptyIndexVisible = value;
    }

    private IParallelWorkspaceManager ParallelWorkspaceManager =>
        _parallelWorkspaceManager ??= LifetimeScope.Resolve<IParallelWorkspaceManager>();

    private IWorkspaceContextMenuManager WorkspaceContextMenuManager =>
        _workspaceContextMenuManager ??= LifetimeScope.Resolve<IWorkspaceContextMenuManager>();

    private void OnWorkspaceFocusChangedEvent(WorkspaceFocusChangedEvent e)
    {
        OnPropertyChanged(nameof(IsFocusedWorkspace));
    }

    private void OnWorkspaceNodeIndexMouseDown(object sender, MouseButtonEventArgs e)
    {
        ParallelWorkspaceManager.SetFocusedWorkspace(WorkspaceNode);
        e.Handled = false;
    }
}
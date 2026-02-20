using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Core;

namespace ZYC.Framework.Workspace;

[Register]
internal sealed partial class WorkspaceMenuView : IDisposable, INotifyPropertyChanged
{
    private IParallelWorkspaceManager? _parallelWorkspaceManager;

    public WorkspaceMenuView(
        ILifetimeScope lifetimeScope,
        WorkspaceMenuConfig workspaceMenuConfig,
        WorkspaceNode workspaceNode,
        IWorkspaceMenuManager workspaceMenuManager)
    {
        LifetimeScope = lifetimeScope;
        WorkspaceMenuConfig = workspaceMenuConfig;
        WorkspaceNode = workspaceNode;
        WorkspaceMenuManager = workspaceMenuManager;

        InitializeComponent();

        WorkspaceMenuConfig.ObserveProperty(nameof(WorkspaceMenuConfig.IsVisible))
            .Throttle(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ =>
            {
                OnPropertyChanged(nameof(IsWorkspaceMenuVisible));
            }).DisposeWith(CompositeDisposable);
    }

    private CompositeDisposable CompositeDisposable { get; } = new();


    private ILifetimeScope LifetimeScope { get; }

    private WorkspaceMenuConfig WorkspaceMenuConfig { get; }

    public bool IsWorkspaceMenuVisible => WorkspaceMenuConfig.IsVisible;

    public WorkspaceNode WorkspaceNode { get; }

    private IWorkspaceMenuManager WorkspaceMenuManager { get; }

    public ObservableCollection<IWorkspaceMenuItem> WorkspaceMenuItems { get; } = new();

    private IParallelWorkspaceManager ParallelWorkspaceManager =>
        _parallelWorkspaceManager ??= LifetimeScope.Resolve<IParallelWorkspaceManager>();

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnWorkspaceMenuViewLoaded(object sender, RoutedEventArgs e)
    {
        WorkspaceMenuItems.Clear();
        var items = WorkspaceMenuManager.GetItems().Reverse();

        foreach (var item in items)
        {
            WorkspaceMenuItems.Add(item);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void OnDropDownButtonClick(object sender, RoutedEventArgs e)
    {
        ParallelWorkspaceManager.SetFocusedWorkspace(WorkspaceNode);
    }
}
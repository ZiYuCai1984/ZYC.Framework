using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Commands;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Workspace;

namespace ZYC.Framework.Tab;

[Register]
internal partial class TabManagerView : INotifyPropertyChanged
{
    private StarCommand? _starCommand;

    public TabManagerView(
        IEventAggregator eventAggregator,
        WorkspaceNode workspaceNode,
        Func<WorkspaceNode, Task> disposeTabManagerViewAsyncFunc,
        ILifetimeScope lifetimeScope,
        TabItemLockState tabItemLockState,
        ITabManager tabManager,
        CloseTabItemCommand closeTabItemCommand)
    {
        EventAggregator = eventAggregator;
        WorkspaceNode = workspaceNode;
        DisposeTabManagerViewAsyncFunc = disposeTabManagerViewAsyncFunc;

        LifetimeScope = lifetimeScope;
        TabItemLockState = tabItemLockState;

        TabManager = tabManager;
        CloseTabItemCommand = closeTabItemCommand;

        NavigationState = tabManager.GetNavigationState(workspaceNode.Id);

        InitializeComponent();

        WorkspaceMenuViewHost.Content = lifetimeScope.Resolve<WorkspaceMenuView>(
            new TypedParameter(typeof(WorkspaceNode), workspaceNode));


        EventAggregator.Subscribe<NavigateCompletedEvent>(OnNavigateCompleted)
            .DisposeWith(CompositeDisposable);

        EventAggregator.Subscribe<TabItemClosedEvent>(OnTabItemClosed)
            .DisposeWith(CompositeDisposable);

        EventAggregator.Subscribe<TabItemsMovedEvent>(OnTabItemsMoved)
            .DisposeWith(CompositeDisposable);

        EventAggregator.Subscribe<WorkspaceFocusChangedEvent>(OnWorkspaceFocusChangedEvent)
            .DisposeWith(CompositeDisposable);

        EventAggregator.Subscribe<TabManagerRestoreCompleted>(_ =>
        {
            PreloadAllTabs(TabControl);
        }).DisposeWith(CompositeDisposable);
    }


    private IEventAggregator EventAggregator { get; }

    private Func<WorkspaceNode, Task> DisposeTabManagerViewAsyncFunc { get; }

    public ILifetimeScope LifetimeScope { get; }

    public TabItemLockState TabItemLockState { get; }

    private NavigationState NavigationState { get; }

    private ITabManager TabManager { get; }

    public string? Uri
    {
        get
        {
            var value = FocusedTabItemInstance?.Uri != null ? FocusedTabItemInstance.Uri.ToString() : string.Empty;
            return value;
        }
        set => _ = StartNavigateAsync(value);
    }

    public string[] NavigateHistory => NavigationState.History.Select(t => t.Uri).ToArray();

    public ObservableCollection<ITabItemInstance> TabItemSource { get; } = new();


    public CloseTabItemCommand CloseTabItemCommand { get; }

    public ITabItemInstance? FocusedTabItemInstance
    {
        get => TabManager.GetFocusedTabItemInstance(WorkspaceNode.Id);
        set => TabManager.SetFocusedTabItemInstance(WorkspaceNode.Id, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public static void PreloadAllTabs(TabControl tab)
    {
        if (tab.Items.Count == 0)
        {
            return;
        }

        var old = tab.SelectedIndex;
        tab.UpdateLayout();

        for (var i = 0; i < tab.Items.Count; i++)
        {
            tab.SelectedIndex = i;
            tab.UpdateLayout();
            Dispatcher.CurrentDispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                }));
        }

        tab.SelectedIndex = old;
        tab.UpdateLayout();
    }

    private void OnTabItemsMoved(TabItemsMovedEvent e)
    {
        if (e.ToWorkspaceId == WorkspaceNode.Id)
        {
            foreach (var instance in e.TabItems)
            {
                if (!TabItemSource.Contains(instance))
                {
                    TabItemSource.Add(instance);
                }
                else
                {
                    Debugger.Break();
                }
            }
        }
        else if (e.FromWorkspaceId == WorkspaceNode.Id)
        {
            foreach (var instance in e.TabItems)
            {
                if (TabItemSource.Contains(instance))
                {
                    TabItemSource.Remove(instance);
                }
                else
                {
                    Debugger.Break();
                }
            }
        }
        else
        {
            return;
        }


        OnPropertyChanged(nameof(Uri));
        OnPropertyChanged(nameof(FocusedTabItemInstance));
        OnPropertyChanged(nameof(NavigateHistory));
    }

    private void OnTabItemClosed(TabItemClosedEvent e)
    {
        if (e.WorkspaceId != WorkspaceNode.Id)
        {
            return;
        }


        var instance = e.TabItemInstance;
        TabItemSource.Remove(instance);

        FocusedTabItemInstance = null;

        OnPropertyChanged(nameof(Uri));
        OnPropertyChanged(nameof(FocusedTabItemInstance));
    }

    private void OnNavigateCompleted(NavigateCompletedEvent e)
    {
        if (e.WorkspaceId != WorkspaceNode.Id)
        {
            return;
        }

        var instance = e.TabItemInstance;
        if (!TabItemSource.Contains(instance))
        {
            TabItemSource.Add(instance);
        }

        OnPropertyChanged(nameof(Uri));
        OnPropertyChanged(nameof(FocusedTabItemInstance));
        OnPropertyChanged(nameof(NavigateHistory));
    }


    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private async Task StartNavigateAsync(string? uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            return;
        }

        if (uri == Uri)
        {
            return;
        }

        if (!System.Uri.TryCreate(uri, UriKind.Absolute, out var result))
        {
            return;
        }

        await TabManager.NavigateAsync(result);
    }

    private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Uri));
    }

    private void OnTabHeaderMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState != MouseButtonState.Pressed
            || e.ChangedButton != MouseButton.Middle)
        {
            return;
        }

        var element = (FrameworkElement)sender;
        CloseTabItemCommand.Execute(element.DataContext);
        e.Handled = true;
    }
}
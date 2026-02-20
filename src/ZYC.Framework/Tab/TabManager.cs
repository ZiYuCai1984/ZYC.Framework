using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Threading;
using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Config;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Abstractions.Workspace;
using ZYC.Framework.Tab.BuildIn;
using Debugger = System.Diagnostics.Debugger;

namespace ZYC.Framework.Tab;

[RegisterSingleInstanceAs(typeof(ITabManager))]
internal partial class TabManager : ITabManager
{
    public TabManager(
        IEventAggregator eventAggregator,
        IParallelWorkspaceManager parallelWorkspaceManager,
        ITabItemFactoryManager tabItemFactoryManager,
        NavigationConfig navigationConfig,
        IAppLogger<TabManager> logger,
        ILifetimeScope lifetimeScope,
        TabItemLockState tabItemLockState)
    {
        ParallelWorkspaceManager = parallelWorkspaceManager;
        TabItemFactoryManager = tabItemFactoryManager;
        NavigationConfig = navigationConfig;
        Logger = logger;
        LifetimeScope = lifetimeScope;
        TabItemLockState = tabItemLockState;

        EventAggregator = eventAggregator;
    }

    private IEventAggregator EventAggregator { get; }

    private IParallelWorkspaceManager ParallelWorkspaceManager { get; }

    private ITabItemFactoryManager TabItemFactoryManager { get; }

    private NavigationConfig NavigationConfig { get; }

    private IAppLogger<TabManager> Logger { get; }

    private ILifetimeScope LifetimeScope { get; }

    private TabItemLockState TabItemLockState { get; }

    private IDictionary<WorkspaceNode, IList<ITabItemInstance>> WorkspaceTabItemInstanceListDictionary { get; } =
        new Dictionary<WorkspaceNode, IList<ITabItemInstance>>();

    private IReadOnlyDictionary<Guid, WorkspaceNode> WorkspaceDictionary =>
        ParallelWorkspaceManager.GetWorkspaceDictionary();

    private IList<ITabItemInstance> TabItemInstances { get; } = new List<ITabItemInstance>();

    private ITabItemFactory[] TabItemFactories => TabItemFactoryManager.GetTabItemFactories();

    /// <summary>
    ///     !WARNING There may be a bug
    /// </summary>
    private IDictionary<Guid, ITabItemInstance?> FocusedTabItemInstances { get; } =
        new ConcurrentDictionary<Guid, ITabItemInstance?>();

    public void MoveTabItemInstance(ITabItemInstance instance, Guid from, Guid to)
    {
        var fromWorkspace = WorkspaceDictionary[from];
        var fromNavigationState = GetNavigationState(from);

        if (fromNavigationState.Focus == instance.Uri)
        {
            fromNavigationState.Focus = null;
        }

        if (!WorkspaceTabItemInstanceListDictionary.ContainsKey(fromWorkspace))
        {
            //ignore
        }
        else
        {
            DetachTabItemInstance(from, instance);
            AttachTabItemInstance(to, instance);

            //TODO SetFocusedTabItemInstance not set for from !!
            //TODO There may be a bug here, which needs to be tested 
            SetFocusedTabItemInstance(to, instance);

            InvokeTabItemsMovedEvent(from, to, [instance]);
        }
    }

    public void MoveAllTabItemInstances(Guid from, Guid to)
    {
        var fromWorkspace = WorkspaceDictionary[from];

        var fromNavigationState = GetNavigationState(from);
        fromNavigationState.Focus = null;
        var fromHistory = fromNavigationState.History.ToList();
        fromNavigationState.History = [];


        if (!WorkspaceTabItemInstanceListDictionary.ContainsKey(fromWorkspace))
        {
            //ignore
        }
        else
        {
            var targetItems = WorkspaceTabItemInstanceListDictionary[fromWorkspace].ToArray();
            if (targetItems.Length != 0)
            {
                foreach (var item in targetItems)
                {
                    DetachTabItemInstance(from, item);
                    AttachTabItemInstance(to, item);
                }

                var toNavigationState = GetNavigationState(to);

                var toHistory = new List<NavigationState.HistoryItem>(toNavigationState.History);
                toHistory.AddRange(fromHistory);
                toNavigationState.History = toHistory.ToArray();

                SetFocusedTabItemInstance(from, null);
            }

            InvokeTabItemsMovedEvent(from, to, targetItems);
        }
    }

    public NavigationState GetNavigationState(Guid workspaceId)
    {
        return WorkspaceDictionary[workspaceId].NavigationState;
    }

    public ITabItemInstance? GetFocusedTabItemInstance(Guid workspaceId)
    {
        // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
        if (!FocusedTabItemInstances.ContainsKey(workspaceId))
        {
            FocusedTabItemInstances.Add(workspaceId, null);
        }

        return FocusedTabItemInstances[workspaceId];
    }


    public ITabItemInstance[] GetTabItemInstances(Guid workspaceId)
    {
        var workspace = WorkspaceDictionary[workspaceId];
        return WorkspaceTabItemInstanceListDictionary[workspace].ToArray();
    }

    public WorkspaceNode GetTabItemInstanceWorkspace(ITabItemInstance instance)
    {
        foreach (var kv in WorkspaceTabItemInstanceListDictionary)
        {
            if (!kv.Value.Contains(instance))
            {
                continue;
            }

            return kv.Key;
        }

        throw new InvalidOperationException();
    }


    public async Task NavigateAsync(Uri uri)
    {
        var workspaceId = ParallelWorkspaceManager.GetFocusedWorkspace().Id;
        await NavigateAsync(workspaceId, uri);
    }

    public async Task NavigateAsync(Guid workspaceId, Uri uri)
    {
        var instance = await InternalNavigateAsync(workspaceId, uri);
        await FocusAsync(instance);
    }


    public async Task TabInternalNavigatingAsync(Uri oriUri, Uri newUri)
    {
        var instance = TabItemInstances.FirstOrDefault(t => t.Uri.Equals(newUri));
        if (instance == null)
        {
            return;
        }

        var navigationState = GetNavigationStateFromTabItemInstance(instance);

        var list = new List<Uri>(navigationState.TabItems);
        var index = list.IndexOf(oriUri);
        if (index == -1)
        {
            return;
        }

        list.Remove(oriUri);
        list.Insert(index, newUri);
        navigationState.TabItems = list.ToArray();

        var workspaceId = GetWorkspaceIdFromTabItemInstance(instance);
        AppendToHistory(workspaceId, newUri);

        await FocusAsync(instance);
    }


    public async Task FocusAsync(Uri uri)
    {
        var instance = TabItemInstances.FirstOrDefault(t => t.Uri == uri);
        if (instance != null)
        {
            await FocusAsync(instance);
        }
    }

    public async Task ReloadAsync(Uri uri)
    {
        var instance = TabItemInstances.FirstOrDefault(t => t.Uri == uri);
        if (instance == null)
        {
            return;
        }

        await CloseAsync(instance);
        await NavigateAsync(uri);
    }

    public async Task CloseAsync(ITabItemInstance instance)
    {
        var workspaceId = GetWorkspaceIdFromTabItemInstance(instance);

        if (TabItemLockState.TabItems.Contains(instance.TabReference))
        {
            return;
        }

        if (!instance.OnClosing())
        {
            return;
        }

        DetachTabItemInstance(workspaceId, instance);

        try
        {
            instance.Dispose();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        InvokeCloseTabItemEvent(workspaceId, instance);

        if (WorkspaceDictionary.TryGetValue(workspaceId, out var sourceWorkspaceNode))
        {
            var instances = WorkspaceTabItemInstanceListDictionary[sourceWorkspaceNode];
            var tab = instances.LastOrDefault();
            if (tab != null)
            {
                await FocusAsync(tab);
            }
            else
            {
                await FocusEmptyAsync(workspaceId);
            }
        }
        else
        {
            Debugger.Break();
            await FocusEmptyAsync(workspaceId);
        }
    }

    public async Task CloseAllAsync()
    {
        var instances = GetAllTabItemInstances();
        foreach (var instance in instances)
        {
            await CloseAsync(instance);
        }
    }

    public async Task RestoreStateAsync()
    {
        var kvs = ParallelWorkspaceManager.GetWorkspaceDictionary();

        foreach (var kv in kvs)
        {
            var workspaceId = kv.Key;
            var navigation = GetNavigationState(kv.Key);

            var backgroundItems = navigation.TabItems.ToArray();
            foreach (var item in backgroundItems)
            {
                await NavigateBackgroundAsync(workspaceId, item);
            }


            var focusUri = navigation.Focus;
            if (focusUri == null)
            {
                continue;
            }

            await FocusAsync(focusUri);
        }


        #region //!WARNING Bad design, used to update and restore the state of <TabItemLockState>

        //If two identical uris are restored, it ignores the order and always locks the previous one!!

        var oriReferences = TabItemLockState.TabItems;
        var newReferences = new List<TabReference>();

        var tabItems = TabItemInstances.ToArray();


        foreach (var oriReference in oriReferences)
        {
            foreach (var item in tabItems)
            {
                if (oriReference.Uri != item.Uri)
                {
                    continue;
                }

                newReferences.Add(item.TabReference);
                break;
            }
        }

        TabItemLockState.TabItems = newReferences.ToArray();

        #endregion
    }

    public void SetFocusedTabItemInstance(Guid workspaceId, ITabItemInstance? instance)
    {
        // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
        if (!FocusedTabItemInstances.ContainsKey(workspaceId))
        {
            FocusedTabItemInstances.Add(workspaceId, null);
        }

        FocusedTabItemInstances[workspaceId] = instance;

        var navigationState = GetNavigationState(workspaceId);
        navigationState.Focus = instance?.Uri;

        InvokeFocusedTabItemChangedEvent(workspaceId, instance);
    }

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


    private ITabItemInstance[] GetAllTabItemInstances()
    {
        return TabItemInstances.ToArray();
    }

    public async Task NavigateBackgroundAsync(Guid workspaceId, Uri uri)
    {
        var instance = await InternalNavigateAsync(workspaceId, uri);
        InvokeNavigateCompletedEvent(workspaceId, instance);
    }


    private async Task FocusEmptyAsync(Guid workspaceId)
    {
        SetFocusedTabItemInstance(workspaceId, null);
        InvokeFocusedTabItemChangedEvent(workspaceId, null);

        await Task.CompletedTask;
    }


    private Task FocusAsync(ITabItemInstance instance)
    {
        var workspaceId = GetWorkspaceIdFromTabItemInstance(instance);
        SetFocusedTabItemInstance(workspaceId, instance);
        InvokeNavigateCompletedEvent(workspaceId, instance);

        return Task.CompletedTask;
    }


    private async Task<ITabItemInstance> InternalNavigateAsync(Guid workspaceId, Uri uri)
    {
        AppendToHistory(workspaceId, uri);

        ITabItemInstance? instance = null;

        try
        {
            var items = TabItemFactories.ToArray();
            foreach (var item in items)
            {
                if (!await item.CheckUriMatchedAsync(uri))
                {
                    continue;
                }

                var instances = TabItemInstances.ToArray();

                //!WARNING Design defeat !!
                if (item.IsSingle
                    && instances.Count(t => UriTools.Equals(t.Uri, uri)) != 0)
                {
                    instance = instances.First(t => UriTools.Equals(t.Uri, uri));
                }
                else
                {
                    //!WARNING LifetimeScope.BeginLifetimeScope() is not used here intentionally because ExternallyOwned is defaults to true.
                    var tabItemCreationContext = new TabItemCreationContext(uri, LifetimeScope);
                    instance = await item.CreateTabItemInstanceAsync(tabItemCreationContext);

                    await instance.LoadAsync();
                    //!WARNING Resolve View in advance, if there are errors, they can be displayed on the page
                    _ = instance.View;

                    AttachTabItemInstance(workspaceId, instance);
                }

                break;
            }

            if (instance == null)
            {
                instance = LifetimeScope.Resolve<NotFoundTabItemInstance>(
                    new TypedParameter(typeof(TabReference), new TabReference(uri)));
                AttachTabItemInstance(workspaceId, instance);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);

            instance = LifetimeScope.Resolve<ErrorTabItemInstance>(
                new TypedParameter(typeof(Exception), e),
                new TypedParameter(typeof(TabReference), new TabReference(uri)));
            AttachTabItemInstance(workspaceId, instance);
        }
        finally
        {
            Debug.Assert(instance != null);
        }

        return instance;
    }

    private void AttachTabItemInstance(Guid workspaceId, ITabItemInstance instance)
    {
        var workspace = WorkspaceDictionary[workspaceId];

        if (!WorkspaceTabItemInstanceListDictionary.ContainsKey(workspace))
        {
            WorkspaceTabItemInstanceListDictionary.Add(workspace, new List<ITabItemInstance>());
        }

        WorkspaceTabItemInstanceListDictionary[workspace].Add(instance);
        TabItemInstances.Add(instance);

        var navigationState = GetNavigationState(workspaceId);
        navigationState.TabItems = GetTabItemUris(workspaceId);
    }

    private Uri[] GetTabItemUris(Guid workspaceId)
    {
        var workspace = WorkspaceDictionary[workspaceId];

        var tabItems = WorkspaceTabItemInstanceListDictionary[workspace]
            .Select(t => t.Uri).ToArray();
        return tabItems;
    }

    private void AppendToHistory(Guid workspaceId, Uri uri)
    {
        var navigationState = GetNavigationState(workspaceId);

        var newList = new List<NavigationState.HistoryItem>(navigationState.History);

        if (newList.Count > NavigationConfig.MaxHistoryNum)
        {
            newList.Remove(newList.Last());
        }

        foreach (var item in newList.ToArray())
        {
            if (item.Uri == uri.ToString())
            {
                newList.Remove(item);
            }
        }

        newList.Insert(0, new NavigationState.HistoryItem
        {
            Time = DateTime.Now,
            Uri = uri.ToString()
        });
        navigationState.History = newList.ToArray();
    }

    /// <summary>
    ///     !WARNING There is no call Dispose !!
    /// </summary>
    private void DetachTabItemInstance(Guid workspaceId, ITabItemInstance instance)
    {
        if (!WorkspaceDictionary.TryGetValue(workspaceId, out var workspace))
        {
            Debugger.Break();
            return;
        }

        WorkspaceTabItemInstanceListDictionary[workspace].Remove(instance);
        TabItemInstances.Remove(instance);

        var navigationState = GetNavigationState(workspaceId);
        navigationState.TabItems = GetTabItemUris(workspaceId);
    }

    private NavigationState GetNavigationStateFromTabItemInstance(ITabItemInstance instance)
    {
        var workspaceId = GetWorkspaceIdFromTabItemInstance(instance);
        return GetNavigationState(workspaceId);
    }

    private WorkspaceNode GetWorkspaceFromTabItemInstance(ITabItemInstance instance)
    {
        foreach (var kv in WorkspaceTabItemInstanceListDictionary)
        {
            if (!kv.Value.Contains(instance))
            {
                continue;
            }

            return kv.Key;
        }

        Debugger.Break();
        throw new InvalidOperationException("");
    }

    private Guid GetWorkspaceIdFromTabItemInstance(ITabItemInstance instance)
    {
        var workspace = GetWorkspaceFromTabItemInstance(instance);
        return workspace.Id;
    }
}
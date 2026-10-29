using System.Collections.Concurrent;
using System.Diagnostics;
using Autofac;
using ZYC.CoreToolkit;
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

    private HashSet<ITabItemInstance> TabItemInstances { get; } = new();

    private Dictionary<ITabItemInstance, ITabItemFactory> TabItemOwners { get; } =
        new(ReferenceEqualityComparer.Instance);

    // Serialize asynchronous opens/reloads to prevent duplicate singleton tabs.
    //!WARNING This non-reentrant gate is held across factory callbacks and LoadAsync.
    // Awaiting external NavigateAsync or ReloadAsync on this manager from those callbacks
    // deadlocks: the nested request waits for the gate, while the outer request waits for
    // the callback. Subsequent opens/reloads are also blocked.
    private SemaphoreSlim NavigationSemaphore { get; } = new(1, 1);

    private ITabItemFactory[] TabItemFactories => TabItemFactoryManager.GetTabItemFactories();

    /// <summary>
    ///     !WARNING There may be a bug
    /// </summary>
    private IDictionary<Guid, ITabItemInstance?> FocusedTabItemInstances { get; } =
        new ConcurrentDictionary<Guid, ITabItemInstance?>();

    public void MoveTabItemInstance(ITabItemInstance instance, Guid from, Guid to)
    {
        var workspaces = WorkspaceDictionary;

        // Validate the move before detaching the tab or changing its navigation state.
        if (!workspaces.TryGetValue(from, out var fromWorkspace)
            || !workspaces.TryGetValue(to, out var toWorkspace)
            || toWorkspace.Left != null
            || toWorkspace.Right != null
            || !WorkspaceTabItemInstanceListDictionary.TryGetValue(fromWorkspace, out var fromItems)
            || !fromItems.Contains(instance))
        {
            return;
        }

        var fromNavigationState = GetNavigationState(from);

        if (fromNavigationState.Focus == instance.Uri)
        {
            fromNavigationState.Focus = null;
        }

        DetachTabItemInstance(from, instance);
        AttachTabItemInstance(to, instance);

        //TODO SetFocusedTabItemInstance not set for from !!
        //TODO There may be a bug here, which needs to be tested
        SetFocusedTabItemInstance(to, instance);

        InvokeTabItemsMovedEvent(from, to, [instance]);
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


    public void MoveTabItemInstance(ITabItemInstance source, ITabItemInstance target, TabInsertPosition position)
    {
        var sourceWorkspace = GetTabItemInstanceWorkspace(source);
        var targetWorkspace = GetTabItemInstanceWorkspace(target);


        if (sourceWorkspace.Id != targetWorkspace.Id)
        {
            DetachTabItemInstance(sourceWorkspace.Id, source);

            var insertIndex = WorkspaceTabItemInstanceListDictionary[targetWorkspace].IndexOf(target);
            if (position == TabInsertPosition.After)
            {
                ++insertIndex;
            }


            AttachTabItemInstance(targetWorkspace.Id, source, insertIndex);

            SetFocusedTabItemInstance(targetWorkspace.Id, source);
            InvokeTabItemsMovedEvent(sourceWorkspace.Id, targetWorkspace.Id, [source], insertIndex);
        }
        else
        {
            //Internal move
            var workspaceId = sourceWorkspace.Id;

            WorkspaceTabItemInstanceListDictionary[sourceWorkspace].Remove(source);

            var insertIndex = WorkspaceTabItemInstanceListDictionary[sourceWorkspace].IndexOf(target);
            if (position == TabInsertPosition.After)
            {
                ++insertIndex;
            }

            WorkspaceTabItemInstanceListDictionary[sourceWorkspace].Insert(insertIndex, source);

            var navigationState = GetNavigationState(workspaceId);
            navigationState.TabItems = GetTabItemUris(workspaceId);

            SetFocusedTabItemInstance(workspaceId, source);
            InvokeTabItemsMovedEvent(workspaceId, workspaceId, [source], insertIndex);
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
        return WorkspaceTabItemInstanceListDictionary.TryGetValue(workspace, out var instances)
            ? instances.ToArray()
            : [];
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

        throw new InvalidOperationException($"No workspace node found for the tab item instance: {instance.Id}");
    }


    public async Task NavigateAsync(Uri uri)
    {
        var workspaceId = ParallelWorkspaceManager.GetFocusedWorkspace().Id;
        await NavigateAsync(workspaceId, uri);
    }

    public async Task NavigateAsync(Guid workspaceId, Uri uri)
    {
        var instance = await InternalNavigateAsync(workspaceId, uri);
        ParallelWorkspaceManager.SetFocusedWorkspace(GetTabItemInstanceWorkspace(instance));
        await FocusAsync(instance);
    }


    public async Task ReloadAsync(ITabItemInstance instance)
    {
        await NavigationSemaphore.WaitAsync();
        try
        {
            await ReloadTabItemAsync(instance);
        }
        finally
        {
            NavigationSemaphore.Release();
        }
    }

    private async Task ReloadTabItemAsync(ITabItemInstance instance)
    {
        if (!TabItemInstances.Contains(instance))
        {
            return;
        }

        if (TabItemLockState.TabItems.Contains(instance.TabReference))
        {
            return;
        }

        if (!instance.OnClosing())
        {
            return;
        }

        var workspaceId = GetWorkspaceIdFromTabItemInstance(instance);
        var workspace = WorkspaceDictionary[workspaceId];
        var insertIndex = WorkspaceTabItemInstanceListDictionary[workspace].IndexOf(instance);
        var uri = instance.Uri;
        var focusedInstance = GetFocusedTabItemInstance(workspaceId);
        var wasFocused = focusedInstance == instance;

        TabItemOwners.Remove(instance, out var factory);

        DetachTabItemInstance(workspaceId, instance);
        StopObservingViewFailures(instance);

        try
        {
            instance.Dispose();
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }

        var existingInstances = TabItemInstances.ToHashSet();
        var newInstance = await CreateOrReuseTabItemAsync(
            workspaceId,
            uri,
            insertIndex,
            factory);

        if (existingInstances.Contains(newInstance))
        {
            // An ownerless error/not-found tab may now route to an existing singleton.
            InvokeCloseTabItemEvent(workspaceId, instance);
            SetFocusedTabItemInstance(workspaceId, null);
            if (!wasFocused && focusedInstance != null)
            {
                await FocusAsync(focusedInstance);
            }
        }
        else
        {
            InvokeTabItemReloadedEvent(workspaceId, instance, newInstance, insertIndex);
        }

        if (wasFocused)
        {
            await FocusAsync(newInstance);
        }
    }

    public async Task NavigateAsync(ITabItemInstance instance, Uri uri)
    {
        if (!TabItemInstances.Contains(instance))
        {
            return;
        }

        if (instance is INavigableTabItemInstance navigable && await navigable.TryNavigateAsync(uri))
        {
            return;
        }

        if (TabItemInstances.Contains(instance) && !UriTools.Equals(instance.Uri, uri))
        {
            await NavigationSemaphore.WaitAsync();
            try
            {
                // The source tab may have closed while another tab was being created.
                if (!TabItemInstances.Contains(instance))
                {
                    return;
                }

                var target = await CreateOrReuseTabItemAsync(GetWorkspaceIdFromTabItemInstance(instance), uri);
                ParallelWorkspaceManager.SetFocusedWorkspace(GetTabItemInstanceWorkspace(target));
                await FocusAsync(target);
            }
            finally
            {
                NavigationSemaphore.Release();
            }
        }
    }

    public Task NotifyUriChangedAsync(ITabItemInstance instance)
    {
        if (!TabItemInstances.Contains(instance))
        {
            return Task.CompletedTask;
        }

        var workspaceId = GetWorkspaceIdFromTabItemInstance(instance);
        var navigationState = GetNavigationState(workspaceId);
        navigationState.TabItems = GetTabItemUris(workspaceId);
        if (GetFocusedTabItemInstance(workspaceId) == instance)
        {
            navigationState.Focus = instance.Uri;
        }

        AppendToHistory(workspaceId, instance.Uri);
        InvokeNavigateCompletedEvent(workspaceId, instance);
        return Task.CompletedTask;
    }


    public async Task FocusAsync(Uri uri)
    {
        foreach (var instance in TabItemInstances)
        {
            if (instance.Uri == uri)
            {
                await FocusAsync(instance);
            }
        }
    }

    public async Task CloseAsync(ITabItemInstance instance)
    {
        if (!TabItemInstances.Contains(instance))
        {
            return;
        }

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
        TabItemOwners.Remove(instance);
        StopObservingViewFailures(instance);

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
        DebuggerTools.CheckCalledOnce();

        var kvs = ParallelWorkspaceManager.GetWorkspaceDictionary();

        foreach (var kv in kvs)
        {
            var workspaceId = kv.Key;
            var navigation = GetNavigationState(kv.Key);

            var backgroundItems = navigation.TabItems.ToArray();
            var focusUri = navigation.Focus;
            ITabItemInstance? focusedInstance = null;
            foreach (var item in backgroundItems)
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                if (item != null)
                {
                    var instance = await InternalNavigateAsync(workspaceId, item);
                    var ownerWorkspaceId = GetWorkspaceIdFromTabItemInstance(instance);
                    InvokeNavigateCompletedEvent(ownerWorkspaceId, instance);
                    if (item == focusUri && ownerWorkspaceId == workspaceId)
                    {
                        focusedInstance = instance;
                    }
                }
            }


            navigation.TabItems = GetTabItemUris(workspaceId);
            if (focusedInstance != null)
            {
                await FocusAsync(focusedInstance);
            }

            navigation.Focus = GetFocusedTabItemInstance(workspaceId)?.Uri;
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

        InvokeTabItemsRestoreCompletedEvent();
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

    private ITabItemInstance[] GetAllTabItemInstances()
    {
        return TabItemInstances.ToArray();
    }

    public async Task NavigateBackgroundAsync(Guid workspaceId, Uri uri)
    {
        var instance = await InternalNavigateAsync(workspaceId, uri);
        InvokeNavigateCompletedEvent(GetWorkspaceIdFromTabItemInstance(instance), instance);
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
        await NavigationSemaphore.WaitAsync();
        try
        {
            return await CreateOrReuseTabItemAsync(workspaceId, uri);
        }
        finally
        {
            NavigationSemaphore.Release();
        }
    }

    private async Task<ITabItemInstance> CreateOrReuseTabItemAsync(
        Guid workspaceId,
        Uri uri,
        int? insertIndex = null,
        ITabItemFactory? factory = null)
    {
        AppendToHistory(workspaceId, uri);

        ITabItemInstance? instance = null;

        try
        {
            var items = factory == null ? TabItemFactories : new[] { factory };
            foreach (var item in items)
            {
                if (factory == null && !await item.CheckUriMatchedAsync(uri))
                {
                    continue;
                }

                factory = item;
                var existing = item.IsSingle
                    ? TabItemOwners.FirstOrDefault(t => ReferenceEquals(t.Value, item)).Key
                    : null;
                if (existing != null)
                {
                    instance = existing;
                }
                else
                {
                    //!WARNING LifetimeScope.BeginLifetimeScope() is not used here intentionally because ExternallyOwned is defaults to true.
                    var tabItemCreationContext = new TabItemCreationContext(uri, LifetimeScope);
                    instance = await item.CreateTabItemInstanceAsync(tabItemCreationContext);

                    await instance.LoadAsync();
                    // Catch construction failures here and observe deferred Component resolution failures.
                    ObserveViewFailures(instance, instance.View);

                    AttachTabItemInstance(workspaceId, instance, insertIndex);
                    TabItemOwners.Add(instance, item);
                }

                break;
            }

            if (instance == null)
            {
                instance = LifetimeScope.Resolve<NotFoundTabItem>(
                    new TypedParameter(typeof(TabReference), new TabReference(uri)));
                AttachTabItemInstance(workspaceId, instance, insertIndex);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);

            if (instance != null)
            {
                StopObservingViewFailures(instance);
                if (TabItemInstances.Contains(instance))
                {
                    DetachTabItemInstance(workspaceId, instance);
                    TabItemOwners.Remove(instance);
                }
                try
                {
                    instance.Dispose();
                }
                catch (Exception disposeException)
                {
                    Logger.Error(disposeException);
                }
            }

            instance = LifetimeScope.Resolve<ErrorTabItem>(
                new TypedParameter(typeof(Exception), e),
                new TypedParameter(typeof(TabReference), new TabReference(uri)));
            AttachTabItemInstance(workspaceId, instance, insertIndex);
            if (factory != null)
            {
                TabItemOwners.Add(instance, factory);
            }
        }
        finally
        {
            Debug.Assert(instance != null);
        }

        return instance;
    }

    private void AttachTabItemInstance(Guid workspaceId, ITabItemInstance instance, int? insertIndex = null)
    {
        var workspace = WorkspaceDictionary[workspaceId];

        if (!WorkspaceTabItemInstanceListDictionary.ContainsKey(workspace))
        {
            WorkspaceTabItemInstanceListDictionary.Add(workspace, new List<ITabItemInstance>());
        }

        if (insertIndex == null)
        {
            WorkspaceTabItemInstanceListDictionary[workspace].Add(instance);
        }
        else
        {
            WorkspaceTabItemInstanceListDictionary[workspace].Insert(insertIndex.Value, instance);
        }

        TabItemInstances.Add(instance);

        var navigationState = GetNavigationState(workspaceId);
        navigationState.TabItems = GetTabItemUris(workspaceId);
    }

    private Uri[] GetTabItemUris(Guid workspaceId)
    {
        return GetTabItemInstances(workspaceId).Select(t => t.Uri).ToArray();
    }

    private void AppendToHistory(Guid workspaceId, Uri uri)
    {
        try
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
        catch (Exception e)
        {
            Debugger.Break();
            Logger.Error(e);
        }
    }

    /// <summary>
    ///     !WARNING There is no call Dispose !!
    /// </summary>
    private void DetachTabItemInstance(Guid workspaceId, ITabItemInstance instance)
    {
        if (!WorkspaceDictionary.TryGetValue(workspaceId, out var workspace))
        {
            return;
        }

        WorkspaceTabItemInstanceListDictionary[workspace].Remove(instance);
        TabItemInstances.Remove(instance);

        var navigationState = GetNavigationState(workspaceId);
        navigationState.TabItems = GetTabItemUris(workspaceId);
    }

    private Guid GetWorkspaceIdFromTabItemInstance(ITabItemInstance instance)
    {
        var workspace = GetTabItemInstanceWorkspace(instance);
        return workspace.Id;
    }
}

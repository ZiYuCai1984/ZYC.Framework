using System.ComponentModel;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions.Event;

namespace ZYC.Framework.Modules.TaskManager.UI;

[Register]
internal sealed partial class TaskManagerView
{
    public TaskManagerView(
        IEventAggregator eventAggregator,
        ITaskManager taskManager,
        TaskManagerState taskManagerState)
    {
        EventAggregator = eventAggregator;
        TaskManager = taskManager;
        TaskManagerState = taskManagerState;

        ManagedTasksCollectionViewSource.Source = ManagedTasks;
        ManagedTasksCollectionViewSource.Filter += OnFilter;

        Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => PropertyChanged += h,
                h => PropertyChanged -= h)
            .Where(e => e.EventArgs.PropertyName == nameof(FilterText)
                        || e.EventArgs.PropertyName == nameof(FilterType))
            .Throttle(TimeSpan.FromMilliseconds(250))
            .DistinctUntilChanged()
            .ObserveOnUI()
            .Subscribe(_ =>
                {
                    RefreshCollectionView();
                }
            ).DisposeWith(CompositeDisposable);
    }

    private IEventAggregator EventAggregator { get; }

    private ITaskManager TaskManager { get; }

    private TaskManagerState TaskManagerState { get; }

    private bool FirstRending { get; set; } = true;


    private void OnManagedCreateFaulted(ManagedCreateFaultedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
        RefreshCollectionView();
    }

    private void OnManagedTaskStatusTextChanged(ManagedTaskStatusTextChangedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
    }

    private void OnManagedTaskStarted(ManagedTaskStartedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
        RefreshCollectionView();
    }

    private void OnManagedTaskResumed(ManagedTaskResumedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
    }

    private void OnManagedTaskProgrssChanged(ManagedTaskProgressChangedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
    }

    private void OnManagedTaskPaused(ManagedTaskPausedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
    }

    private void OnManagedTaskFaulted(ManagedTaskFaultedEvnet obj)
    {
        UpdateSnapshot(obj.Snapshot);
        RefreshCollectionView();
    }

    private void OnManagedTaskCompleted(ManagedTaskCompletedEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
        RefreshCollectionView();
    }

    private void OnManagedTaskClearUpCompleted(ManagedTaskClearUpCompletedEvent obj)
    {
        var toRemove = ManagedTasks.Where(t => obj.TaskIds.Contains(t.Id)).ToArray();

        foreach (var r in toRemove)
        {
            ManagedTasks.Remove(r);
        }

        RefreshCollectionView();
    }

    private void OnManagedTaskCanceled(ManagedTaskCanceledEvent obj)
    {
        UpdateSnapshot(obj.Snapshot);
    }

    private void UpdateSnapshot(TaskRecord snapshot)
    {
        var manageTask = ManagedTasks.FirstOrDefault(m => m.Id == snapshot.Id);
        if (manageTask == null)
        {
            DebuggerTools.Break();
            return;
        }

        manageTask.UpdateSnapshot(snapshot);
    }


    private void SubscribeEvent<TEvent>(Action<TEvent> handler) where TEvent : notnull
    {
        EventAggregator.Subscribe(handler, true)
            .DisposeWith(CompositeDisposable);
    }

    private void OnManagedTaskEnqueued(ManagedTaskEnqueuedEvent obj)
    {
        ManagedTasks.Add(new ManagedTaskWrapper(obj.Snapshot));
        RefreshCollectionView();
    }

    public override void Dispose()
    {
        base.Dispose();

        CompositeDisposable.Dispose();
    }

    private void OnTaskManagerViewLoaded(object sender, RoutedEventArgs e)
    {
        if (!FirstRending)
        {
            return;
        }

        FirstRending = false;


        ManagedTasks.Clear();
        var tasks = TaskManager.GetAllTasks();
        foreach (var t in tasks)
        {
            ManagedTasks.Add(new ManagedTaskWrapper(t.Snapshot));
        }

        RefreshCollectionView();


        SubscribeEvent<ManagedCreateFaultedEvent>(OnManagedCreateFaulted);
        SubscribeEvent<ManagedTaskCanceledEvent>(OnManagedTaskCanceled);
        SubscribeEvent<ManagedTaskClearUpCompletedEvent>(OnManagedTaskClearUpCompleted);
        SubscribeEvent<ManagedTaskCompletedEvent>(OnManagedTaskCompleted);
        SubscribeEvent<ManagedTaskEnqueuedEvent>(OnManagedTaskEnqueued);
        SubscribeEvent<ManagedTaskFaultedEvnet>(OnManagedTaskFaulted);
        SubscribeEvent<ManagedTaskPausedEvent>(OnManagedTaskPaused);
        SubscribeEvent<ManagedTaskProgressChangedEvent>(OnManagedTaskProgrssChanged);
        SubscribeEvent<ManagedTaskResumedEvent>(OnManagedTaskResumed);
        SubscribeEvent<ManagedTaskStartedEvent>(OnManagedTaskStarted);
        SubscribeEvent<ManagedTaskStatusTextChangedEvent>(OnManagedTaskStatusTextChanged);
    }


    public class ManagedTaskWrapper : INotifyPropertyChanged
    {
        public ManagedTaskWrapper(TaskRecord snapshot)
        {
            Snapshot = snapshot;
        }

        public Guid Id => Snapshot.Id;

        public TaskRecord Snapshot { get; private set; }

        public double? Progress => Snapshot.Progress;

        public string? StatusText => Snapshot.StatusText;

        public string ProviderId => Snapshot.ProviderId;

        public string DefinitionId => Snapshot.DefinitionId;

        public int DefinitionVersion => Snapshot.DefinitionVersion;

        public PauseKind PauseKind => Snapshot.PauseKind;

        public ManagedTaskState State => Snapshot.State;

        public string? FaultText => Snapshot.FaultText;

        public DateTimeOffset CreatedAt => Snapshot.CreatedAt;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void UpdateSnapshot(TaskRecord snapshot)
        {
            Snapshot = snapshot;

            OnPropertyChanged(nameof(Snapshot));
            OnPropertyChanged(nameof(Progress));
            OnPropertyChanged(nameof(State));
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(ProviderId));
            OnPropertyChanged(nameof(DefinitionId));
            OnPropertyChanged(nameof(DefinitionVersion));
            OnPropertyChanged(nameof(PauseKind));
            OnPropertyChanged(nameof(FaultText));
            OnPropertyChanged(nameof(CreatedAt));
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
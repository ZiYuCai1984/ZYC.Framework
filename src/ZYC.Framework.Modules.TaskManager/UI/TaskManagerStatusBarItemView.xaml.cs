using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Runtime.CompilerServices;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions.Event;

namespace ZYC.Framework.Modules.TaskManager.UI;

[RegisterSingleInstance]
internal partial class TaskManagerStatusBarItemView : IDisposable, INotifyPropertyChanged
{
    public TaskManagerStatusBarItemView(IEventAggregator eventAggregator, ITaskManager taskManager)
    {
        EventAggregator = eventAggregator;
        TaskManager = taskManager;

        InitializeComponent();

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

    private CompositeDisposable CompositeDisposable { get; } = new();

    private IEventAggregator EventAggregator { get; }

    private ITaskManager TaskManager { get; }

    public Uri Uri => TaskManagerModuleConstants.Uri;

    public string RunningTaskNum
    {
        get
        {
            var tasks = TaskManager.GetAllTasks();

            var num = tasks.Count(t => t.Snapshot.State == ManagedTaskState.Running
                                       || t.Snapshot.State == ManagedTaskState.Queuing);
            if (num == 0)
            {
                return "";
            }

            return num.ToString();
        }
    }

    public void Dispose()
    {
        CompositeDisposable.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnManagedCreateFaulted(ManagedCreateFaultedEvent obj)
    {
        OnPropertyChanged(nameof(RunningTaskNum));
    }

    private void OnManagedTaskStatusTextChanged(ManagedTaskStatusTextChangedEvent obj)
    {
        //ignore
    }

    private void OnManagedTaskStarted(ManagedTaskStartedEvent obj)
    {
        //ignore
    }

    private void OnManagedTaskResumed(ManagedTaskResumedEvent obj)
    {
        //ignore
    }

    private void OnManagedTaskProgrssChanged(ManagedTaskProgressChangedEvent obj)
    {
        //ignore
    }

    private void SubscribeEvent<TEvent>(Action<TEvent> handler) where TEvent : notnull
    {
        EventAggregator.Subscribe(handler)
            .DisposeWith(CompositeDisposable);
    }

    private void OnManagedTaskPaused(ManagedTaskPausedEvent obj)
    {
        //ignore
    }

    private void OnManagedTaskFaulted(ManagedTaskFaultedEvnet obj)
    {
        OnPropertyChanged(nameof(RunningTaskNum));
    }

    private void OnManagedTaskCanceled(ManagedTaskCanceledEvent obj)
    {
        OnPropertyChanged(nameof(RunningTaskNum));
    }

    private void OnManagedTaskCompleted(ManagedTaskCompletedEvent obj)
    {
        OnPropertyChanged(nameof(RunningTaskNum));
    }

    private void OnManagedTaskClearUpCompleted(ManagedTaskClearUpCompletedEvent obj)
    {
        //ignore
    }

    private void OnManagedTaskEnqueued(ManagedTaskEnqueuedEvent obj)
    {
        OnPropertyChanged(nameof(RunningTaskNum));
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
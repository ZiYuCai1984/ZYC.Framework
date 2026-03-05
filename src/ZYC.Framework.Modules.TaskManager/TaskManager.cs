using System.Collections.Concurrent;
using System.Threading.Channels;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.TaskManager.Abstractions.Event;

namespace ZYC.Framework.Modules.TaskManager;

[RegisterSingleInstanceAs(typeof(ITaskManager))]
internal sealed class TaskManager : ITaskManager, IAsyncDisposable
{
    private readonly TaskCatalog _catalog;

    private readonly TaskManagerConfig _config;

    private readonly object _gate = new();

    // runtime controls per running task
    private readonly ConcurrentDictionary<Guid, PauseTokenSource> _pauseSources = new();

    private readonly Dictionary<Guid, TaskRecord> _records = new();

    private readonly Channel<Guid> _runQueue = Channel.CreateUnbounded<Guid>(
        new UnboundedChannelOptions { SingleReader = false, SingleWriter = false });

    // prevent duplicate schedule
    private readonly ConcurrentDictionary<Guid, byte> _scheduled = new();

    private readonly CancellationTokenSource _shutdown = new();

    private readonly ITaskStore _store;

    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _taskCts = new();

    private readonly Dictionary<Guid, ManagedTask> _tasks = new();

    private readonly List<Task> _workers = new();

    public TaskManager(
        ITaskStore store,
        TaskCatalog catalog,
        TaskManagerConfig config,
        IEventAggregator eventAggregator)
    {
        EventAggregator = eventAggregator;
        _store = store;
        _catalog = catalog;
        _config = config;
    }

    private IEventAggregator EventAggregator { get; }

    public async ValueTask DisposeAsync()
    {
        await _shutdown.CancelAsync();
        _runQueue.Writer.TryComplete();

        try
        {
            await Task.WhenAll(_workers);
        }
        catch
        {
            /* ignore */
        }

        _shutdown.Dispose();
        foreach (var kv in _taskCts)
        {
            kv.Value.Dispose();
        }
    }

    public IReadOnlyList<IManagedTask> GetAllTasks()
    {
        lock (_gate)
        {
            return _tasks.Values.Cast<IManagedTask>().ToList();
        }
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        DebuggerTools.CheckCalledOnce();

        var loaded = await _store.LoadAllAsync(ct);
        var changed = new List<TaskRecord>(loaded.Count);

        foreach (var r in loaded)
        {
            var fixedRecord = r;

            //On process restart: Running or (Paused from Running) cannot be guaranteed to continue -> mark Faulted.

            if (r.State == ManagedTaskState.Running ||
                (r.State == ManagedTaskState.Paused && r.PauseKind == PauseKind.PausedWhileRunning))
            {
                fixedRecord = r with
                {
                    PauseKind = PauseKind.None,
                    State = ManagedTaskState.Faulted,
                    EndedAt = DateTimeOffset.Now,
                    FaultText = $"Interrupted by shutdown at {DateTimeOffset.Now:O}"
                };
            }

            changed.Add(fixedRecord);
        }

        lock (_gate)
        {
            foreach (var r in changed)
            {
                _records[r.Id] = r;
                var task = new ManagedTask(this, r);
                _tasks[r.Id] = task;
            }
        }

        // Persist any fixes
        await FlushAsync(ct);

        // Start workers
        for (var i = 0; i < _config.MaxConcurrency; i++)
        {
            _workers.Add(Task.Run(() => WorkerLoopAsync(_shutdown.Token)));
        }

        if (_config.AutoStartQueuedOnLoad)
        {
            foreach (var r in changed)
            {
                if (r.State == ManagedTaskState.Queuing)
                {
                    Schedule(r.Id);
                }
            }
        }
    }

    public IManagedTask Enqueue(string providerId, string definitionId, int version, string payloadJson)
    {
        var record = new TaskRecord
        {
            Id = Guid.NewGuid(),
            ProviderId = providerId,
            DefinitionId = definitionId,
            DefinitionVersion = version,
            PayloadJson = payloadJson,
            State = ManagedTaskState.Queuing,
            PauseKind = PauseKind.None,
            CreatedAt = DateTimeOffset.Now
        };

        ManagedTask task;
        lock (_gate)
        {
            _records[record.Id] = record;
            task = new ManagedTask(this, record);
            _tasks[record.Id] = task;
        }

        _ = FlushAsync(CancellationToken.None);
        EventAggregator.Publish(new ManagedTaskEnqueuedEvent(record));

        if (_config.AutoStartOnEnqueue)
        {
            Schedule(record.Id);
        }

        return task;
    }

    public async Task StartAsync(Guid taskId, CancellationToken ct = default)
    {
        TaskRecord r;
        lock (_gate)
        {
            if (!_records.TryGetValue(taskId, out r!))
            {
                throw new InvalidOperationException($"Task not found: {taskId}");
            }
        }

        if (r.State == ManagedTaskState.Paused)
        {
            throw new InvalidOperationException("Task is paused. Call Resume() first.");
        }

        if (r.State is ManagedTaskState.Completed or ManagedTaskState.Faulted or ManagedTaskState.Canceled)
        {
            throw new InvalidOperationException("Task is already finished. Create a new task to run again.");
        }

        // Queuing: just ensure scheduled
        Schedule(taskId);

        EventAggregator.Publish(new ManagedTaskStartedEvent(r));
        await Task.CompletedTask;
    }

    public async Task PauseAsync(Guid taskId, CancellationToken ct = default)
    {
        TaskRecord r;
        lock (_gate)
        {
            if (!_records.TryGetValue(taskId, out r!))
            {
                throw new InvalidOperationException($"Task not found: {taskId}");
            }
        }

        if (r.State == ManagedTaskState.Paused)
        {
            return;
        }

        if (r.State == ManagedTaskState.Queuing)
        {
            // pause while queued: stays not runnable
            var nr = r with { State = ManagedTaskState.Paused, PauseKind = PauseKind.PausedWhileQueuing };
            UpdateRecord(nr);
            await FlushAsync(ct);
            EventAggregator.Publish(new ManagedTaskPausedEvent(nr));
            return;
        }

        if (r.State != ManagedTaskState.Running)
        {
            throw new InvalidOperationException($"Cannot pause in state: {r.State}");
        }

        // pause while running: cooperative pause token
        var pauseSrc = _pauseSources.GetOrAdd(taskId, _ => new PauseTokenSource());
        pauseSrc.Pause();

        var paused = r with { State = ManagedTaskState.Paused, PauseKind = PauseKind.PausedWhileRunning };
        UpdateRecord(paused);

        EventAggregator.Publish(new ManagedTaskPausedEvent(paused));
        await FlushAsync(ct);
    }

    public async Task ResumeAsync(Guid taskId, CancellationToken ct = default)
    {
        TaskRecord r;
        lock (_gate)
        {
            if (!_records.TryGetValue(taskId, out r!))
            {
                throw new InvalidOperationException($"Task not found: {taskId}");
            }
        }

        if (r.State != ManagedTaskState.Paused)
        {
            throw new InvalidOperationException($"Cannot resume in state: {r.State}");
        }

        if (r.PauseKind == PauseKind.PausedWhileQueuing)
        {
            var nr = r with { State = ManagedTaskState.Queuing, PauseKind = PauseKind.None };
            UpdateRecord(nr);
            await FlushAsync(ct);

            // resume => back to queue
            Schedule(taskId);

            EventAggregator.Publish(new ManagedTaskResumedEvent(nr));
            return;
        }

        if (r.PauseKind == PauseKind.PausedWhileRunning)
        {
            var pauseSrc = _pauseSources.GetOrAdd(taskId, _ => new PauseTokenSource());
            pauseSrc.Resume();

            // back to Running state (worker still owns execution)
            var nr = r with { State = ManagedTaskState.Running, PauseKind = PauseKind.None };
            UpdateRecord(nr);
            await FlushAsync(ct);

            EventAggregator.Publish(new ManagedTaskResumedEvent(nr));
            return;
        }

        DebuggerTools.Break();

        throw new InvalidOperationException("Paused task has unknown ActiveState.");
    }


    public async Task<Guid[]> ClearUpTasksAsync(CancellationToken ct = default)
    {
        List<Guid> toRemove;

        // 1) Decide which to delete inside the lock + remove from core dictionaries (consistency).
        lock (_gate)
        {
            toRemove = _records.Values
                .Where(r => r.State is ManagedTaskState.Completed or ManagedTaskState.Faulted
                    or ManagedTaskState.Canceled)
                .Select(r => r.Id)
                .ToList();

            foreach (var id in toRemove)
            {
                _records.Remove(id);
                _tasks.Remove(id);
            }
        }

        if (toRemove.Count == 0)
        {
            return [];
        }

        // 2) Clean up runtime resources outside the lock (Dispose can be slow; don't hold _gate).
        foreach (var id in toRemove)
        {
            _scheduled.TryRemove(id, out _);

            if (_taskCts.TryRemove(id, out var cts))
            {
                cts.Dispose();
            }

            _pauseSources.TryRemove(id, out _);
        }

        // 3) Persist: write back to tasks.json.
        await FlushAsync(ct);

        var reuslt = toRemove.ToArray();
        EventAggregator.Publish(new ManagedTaskClearUpCompletedEvent(reuslt));
        return reuslt.ToArray();
    }

    private void Schedule(Guid taskId)
    {
        if (_scheduled.TryAdd(taskId, 0))
        {
            _runQueue.Writer.TryWrite(taskId);
        }
    }

    private async Task WorkerLoopAsync(CancellationToken ct)
    {
        while (await _runQueue.Reader.WaitToReadAsync(ct))
        {
            while (_runQueue.Reader.TryRead(out var taskId))
            {
                _scheduled.TryRemove(taskId, out _);

                TaskRecord r;
                lock (_gate)
                {
                    if (!_records.TryGetValue(taskId, out r!))
                    {
                        continue;
                    }
                }

                if (r.State != ManagedTaskState.Queuing)
                {
                    continue; // paused/finished/etc.
                }

                // Create definition
                if (!_catalog.TryCreate(r, out var def, out var err) || def is null)
                {
                    var faulted = r with
                    {
                        PauseKind = PauseKind.None,
                        State = ManagedTaskState.Faulted,
                        EndedAt = DateTimeOffset.Now,
                        FaultText = err
                    };
                    UpdateRecord(faulted);
                    await FlushAsync(CancellationToken.None);

                    EventAggregator.Publish(new ManagedCreateFaultedEvent(faulted));

                    continue;
                }

                // Run it
                await RunOneAsync(r, def, ct);
            }
        }
    }

    private async Task RunOneAsync(TaskRecord r, IManagedTaskDefinition def, CancellationToken managerCt)
    {
        // per-task cts
        var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(managerCt);
        _taskCts[r.Id] = linkedCts;

        var pauseSrc = _pauseSources.GetOrAdd(r.Id, _ => new PauseTokenSource());
        pauseSrc.Resume(); // ensure not paused at start

        var started = r with
        {
            PauseKind = PauseKind.None,
            State = ManagedTaskState.Running,
            StartedAt = DateTimeOffset.Now,
            StatusText = "Running..."
        };
        UpdateRecord(started);
        await FlushAsync(CancellationToken.None);

        // Throttle persistence for progress
        var lastPersistTick = Environment.TickCount64;

        void ReportProgress(double p)
        {
            var cur = GetRecord(r.Id);
            var nr = cur with { Progress = Math.Clamp(p, 0, 1) };
            UpdateRecord(nr);

            var now = Environment.TickCount64;
            if (now - lastPersistTick >= 250)
            {
                lastPersistTick = now;
                _ = FlushAsync(CancellationToken.None);
            }

            EventAggregator.Publish(new ManagedTaskProgressChangedEvent(nr));
        }

        void ReportStatus(string s)
        {
            var cur = GetRecord(r.Id);
            var nr = cur with { StatusText = s };
            UpdateRecord(nr);
            _ = FlushAsync(CancellationToken.None);

            EventAggregator.Publish(new ManagedTaskStatusTextChangedEvent(nr));
        }

        var ctx = new TaskExecutionContext
        {
            TaskId = r.Id,
            Pause = pauseSrc.Token,
            Progress = new Progress<double>(ReportProgress),
            StatusText = new Progress<string>(ReportStatus)
        };

        try
        {
            await def.ExecuteAsync(ctx, linkedCts.Token);

            // ✅ Check for cancellation at the end of the success path.
            var curAfter = GetRecord(r.Id);
            if (linkedCts.IsCancellationRequested || curAfter.State == ManagedTaskState.Canceled)
            {
                throw new OperationCanceledException(linkedCts.Token);
            }

            var completed = GetRecord(r.Id) with
            {
                PauseKind = PauseKind.None,
                State = ManagedTaskState.Completed,
                EndedAt = DateTimeOffset.Now,
                StatusText = "Completed."
            };
            UpdateRecord(completed);
            _tasks[completed.Id].TrySetCompleted();
            await FlushAsync(CancellationToken.None);

            EventAggregator.Publish(new ManagedTaskCompletedEvent(completed));
        }
        catch (OperationCanceledException oce)
        {
            var cur = GetRecord(r.Id);
            var faultText = cur.FaultText;
            if (string.IsNullOrWhiteSpace(faultText))
            {
                faultText = $"Canceled: {oce.Message}";
            }

            var faulted = cur with
            {
                PauseKind = PauseKind.None,
                State = ManagedTaskState.Canceled,
                EndedAt = DateTimeOffset.Now,
                StatusText = "Canceled.",
                FaultText = faultText
            };


            UpdateRecord(faulted);
            _tasks[faulted.Id].TrySetCanceled();
            await FlushAsync(CancellationToken.None);

            EventAggregator.Publish(new ManagedTaskCanceledEvent(faulted));
        }
        catch (Exception ex)
        {
            var faulted = GetRecord(r.Id) with
            {
                PauseKind = PauseKind.None,
                State = ManagedTaskState.Faulted,
                EndedAt = DateTimeOffset.Now,
                FaultText = ex.ToString()
            };
            UpdateRecord(faulted);
            _tasks[faulted.Id].TrySetFaulted(ex);
            await FlushAsync(CancellationToken.None);

            EventAggregator.Publish(new ManagedTaskFaultedEvnet(faulted, ex));
        }
        finally
        {
            if (_taskCts.TryRemove(r.Id, out var v))
            {
                v.Dispose();
            }
        }
    }

    private TaskRecord GetRecord(Guid id)
    {
        lock (_gate)
        {
            return _records[id];
        }
    }

    private void UpdateRecord(TaskRecord record)
    {
        lock (_gate)
        {
            _records[record.Id] = record;
            _tasks[record.Id].SetRecord(record);
        }
    }

    private async Task FlushAsync(CancellationToken ct)
    {
        List<TaskRecord> snapshot;
        lock (_gate)
        {
            snapshot = _records.Values.OrderBy(r => r.CreatedAt).ToList();
        }

        await _store.SaveAllAsync(snapshot, ct);
    }

    public async Task CancelAsync(Guid taskId, CancellationToken ct = default)
    {
        TaskRecord r;
        lock (_gate)
        {
            if (!_records.TryGetValue(taskId, out r!))
            {
                throw new InvalidOperationException($"Task not found: {taskId}");
            }
        }

        // Already terminal
        if (r.State is ManagedTaskState.Completed or ManagedTaskState.Faulted or ManagedTaskState.Canceled)
        {
            return;
        }

        // 1) Not started yet (Queuing / PausedWhileQueuing) => mark as Canceled immediately.
        if (r.State == ManagedTaskState.Queuing ||
            (r.State == ManagedTaskState.Paused && r.PauseKind == PauseKind.PausedWhileQueuing))
        {
            // optional: remove schedule mark to avoid re-queue
            _scheduled.TryRemove(taskId, out _);

            var nr = r with
            {
                State = ManagedTaskState.Canceled,
                PauseKind = PauseKind.None,
                EndedAt = DateTimeOffset.Now,
                FaultText = "Canceled before start."
            };

            UpdateRecord(nr);
            EventAggregator.Publish(new ManagedTaskCanceledEvent(nr));
            await FlushAsync(ct);
            return;
        }

        // 2) Running (Running / PausedWhileRunning) => cooperative cancellation.
        if (r.State == ManagedTaskState.Running ||
            (r.State == ManagedTaskState.Paused && r.PauseKind == PauseKind.PausedWhileRunning))
        {
            // Let the task reach a cancellation point quickly:
            // - If paused, resume the gate first (otherwise it may be stuck in WaitIfPaused).
            if (r.State == ManagedTaskState.Paused)
            {
                var pauseSrc = _pauseSources.GetOrAdd(taskId, _ => new PauseTokenSource());
                pauseSrc.Resume();
            }

            // Signal cancellation.
            if (_taskCts.TryGetValue(taskId, out var cts))
            {
                // ReSharper disable once MethodHasAsyncOverload
                //!WARNING Synchronization is required here !!
                cts.Cancel();
            }

            var nr = r with
            {
                PauseKind = PauseKind.None,
                State = ManagedTaskState.Canceled,
                EndedAt = DateTimeOffset.Now,
                StatusText = "Cancel requested.",
                FaultText = "Cancel requested."
            };

            UpdateRecord(nr);
            EventAggregator.Publish(new ManagedTaskCanceledEvent(nr));
            await FlushAsync(ct);
            return;
        }

        // Other states are not supported.
        throw new InvalidOperationException($"Cannot cancel in state: {r.State} (PauseKind={r.PauseKind})");
    }
}

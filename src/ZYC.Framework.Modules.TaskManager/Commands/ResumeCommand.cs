using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager.Commands;

[RegisterSingleInstanceAs(typeof(ResumeCommand), typeof(ITaskOperationCommand), PreserveExistingDefaults = true)]
internal class ResumeCommand : AsyncCommandBase<Guid?>, ITaskOperationCommand
{
    public ResumeCommand(ILifetimeScope lifetimeScope)
    {
        LifetimeScope = lifetimeScope;
    }

    private ILifetimeScope LifetimeScope { get; }

    private ITaskManager TaskManager => LifetimeScope.Resolve<ITaskManager>();

    protected override async Task InternalExecuteAsync(Guid? taskId)
    {
        if (taskId == null)
        {
            DebuggerTools.Break();
            return;
        }

        if (TaskManager.TryGetTask(taskId.Value, out var task))
        {
            await task.ResumeAsync();
        }

        LifetimeScope.RaiseTaskOperationCommandsCanExecuteChanged();
    }

    protected override bool InternalCanExecute(Guid? taskId)
    {
        if (taskId == null)
        {
            return false;
        }

        if (!TaskManager.TryGetTask(taskId.Value, out var task))
        {
            return false;
        }

        return task.Snapshot.State == ManagedTaskState.Paused;
    }
}
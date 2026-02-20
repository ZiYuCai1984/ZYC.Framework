using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.TaskManager.Commands;

[RegisterSingleInstance]
internal class ClearUpTasksCommand : AsyncCommandBase
{
    public ClearUpTasksCommand(ITaskManager taskManager)
    {
        TaskManager = taskManager;
    }

    private ITaskManager TaskManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        await TaskManager.ClearUpTasksAsync();
    }
}
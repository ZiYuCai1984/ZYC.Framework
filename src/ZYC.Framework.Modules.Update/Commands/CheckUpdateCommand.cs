using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions.Commands;

namespace ZYC.Framework.Modules.Update.Commands;

[RegisterSingleInstanceAs(typeof(CheckUpdateCommand), typeof(ICheckUpdateCommand))]
internal class CheckUpdateCommand : CommandBase, ICheckUpdateCommand
{
    public CheckUpdateCommand(
        ITaskManager taskManager,
        IUpdateManager updateManager)
    {
        TaskManager = taskManager;
        UpdateManager = updateManager;
    }

    private ITaskManager TaskManager { get; }

    private IUpdateManager UpdateManager { get; }

    protected override void InternalExecute(object? parameter)
    {
        TaskManager.Enqueue(
            UpdateTaskProvider.ProviderId,
            CheckUpdateTaskDefinition.DefinitionId,
            1,
            "");
    }
}
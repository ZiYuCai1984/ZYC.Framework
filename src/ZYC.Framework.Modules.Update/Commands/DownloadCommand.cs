using System.Diagnostics;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TaskManager.Abstractions;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update.Commands;

[RegisterSingleInstance]
internal class DownloadCommand : AsyncCommandBase
{
    public DownloadCommand(ITaskManager taskManager, IUpdateManager updateManager)
    {
        TaskManager = taskManager;
        UpdateManager = updateManager;
    }

    private ITaskManager TaskManager { get; }
    private IUpdateManager UpdateManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        var product = UpdateManager.GetCurrentUpdateContext().NewProduct;
        Debug.Assert(product != null);

        var managedTask = TaskManager.Enqueue(
            UpdateTaskProvider.ProviderId,
            DownloadNewProductDefinition.DefinitionId, 1, product.ToJsonText());

        await managedTask.Completion;
    }

    public override bool CanExecute(object? parameter)
    {
        return !IsExecuting;
    }
}
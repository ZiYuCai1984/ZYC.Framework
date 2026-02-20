using System.Text.Json;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.Mock.Commands;

[RegisterSingleInstance]
internal class CountTaskCommand : CommandBase<int>
{
    public CountTaskCommand(ITaskManager taskManager)
    {
        TaskManager = taskManager;
    }

    private ITaskManager TaskManager { get; }

    protected override void InternalExecute(int parameter)
    {
        var payload = JsonSerializer.Serialize(new { Steps = 60, DelayMs = 100 });
        for (var i = 0; i < parameter; ++i)
        {
            TaskManager.Enqueue("mock", "mock/count", 1, payload);
        }
    }
}
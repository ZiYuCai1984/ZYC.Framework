using System.Text.Json;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Modules.TaskManager.Abstractions;

namespace ZYC.Framework.Modules.Mock;

[Register]
internal sealed class CountTaskDefinition : IManagedTaskDefinition
{
    public CountTaskDefinition(TaskDefinitionCreateContext taskDefinitionCreateContext)
    {
        CountPayload = JsonSerializer.Deserialize<CountPayload>(taskDefinitionCreateContext.PayloadJson)
                       ?? throw new InvalidOperationException("Invalid payload JSON.");
    }

    private CountPayload CountPayload { get; }

    public int Version => 1;

    public string TaskType => "count";

    public string DisplayName => "Count Task";

    public string? Description => "Counts 1..N with delay; supports pause/resume.";

    public async Task ExecuteAsync(TaskExecutionContext context, CancellationToken ct)
    {
        var steps = Math.Max(1, CountPayload.Steps);
        var delay = Math.Max(0, CountPayload.DelayMs);

        context.StatusText?.Report($"Start counting: {steps} steps");

        for (var i = 0; i < steps; i++)
        {
            ct.ThrowIfCancellationRequested();
            await context.Pause.WaitIfPausedAsync(ct); // cooperative pause point

            await Task.Delay(delay, ct);

            var progress = (i + 1) / (double)steps;
            context.Progress?.Report(progress);
            context.StatusText?.Report($"Step {i + 1}/{steps}");
        }
    }
}
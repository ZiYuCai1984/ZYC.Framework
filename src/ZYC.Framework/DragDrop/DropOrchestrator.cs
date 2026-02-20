using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.DragDrop;

namespace ZYC.Framework.DragDrop;

/// <summary>
///     Default orchestrator:
///     - Collect actions from all providers (isolated, robust)
///     - Filter CanRun
///     - Sort by IsDefault then Priority
///     - Decision:
///     - 0 => None
///     - 1 => ExecuteDefault
///     - multiple => if has default and not holding Ctrl => ExecuteDefault else PickAction
/// </summary>
[RegisterSingleInstanceAs(typeof(IDropOrchestrator))]
internal sealed class DropOrchestrator : IDropOrchestrator
{
    private readonly IDropActionProvider[] _providers;

    public DropOrchestrator(
        IDropActionProvider[] providers, 
        ILogger<DropOrchestrator> logger)
    {
        Logger = logger;
        _providers = providers;
    }

    private ILogger<DropOrchestrator> Logger { get; }

    public async Task<DropResolution> ResolveAsync(DropPayload payload, DropContext context)
    {
        var frozen = new DropPayload((string[])payload.Paths.Clone(), payload.Extras);

        var tasks = _providers.Select(p => SafeGetActionsAsync(p, frozen, context)).ToArray();
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        var all = results.SelectMany(x => x)
            .Where(a => a is not null)
            .Where(a => a.CanRun())
            .ToList();

        // Optional: de-dup by Id (keep best one)
        all = all.GroupBy(a => a.Id, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.Priority)
                .First())
            .ToList();

        var ordered = all.OrderByDescending(a => a.IsDefault)
            .ThenByDescending(a => a.Priority)
            .ThenBy(a => a.Title, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (ordered.Length == 0)
        {
            return new DropResolution([], DropResolutionMode.None, null);
        }

        if (ordered.Length == 1)
        {
            return new DropResolution(ordered, DropResolutionMode.ExecuteDefault, ordered[0]);
        }

        var defaultAction = ordered.FirstOrDefault(a => a.IsDefault);

        // Policy: Ctrl forces picker
        var ctrlHeld = (context.Modifiers & ModifierKeys.Ctrl) != 0;

        if (defaultAction is not null && !ctrlHeld)
        {
            return new DropResolution(ordered, DropResolutionMode.ExecuteDefault, defaultAction);
        }

        return new DropResolution(ordered, DropResolutionMode.PickAction, defaultAction);
    }

    private async Task<DropAction[]> SafeGetActionsAsync(
        IDropActionProvider provider,
        DropPayload payload,
        DropContext context)
    {
        try
        {
            var actions = await provider.GetActionsAsync(payload, context).ConfigureAwait(false);
            return actions;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            return [];
        }
    }
}
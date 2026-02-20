using ZYC.CoreToolkit.Extensions.Autofac.Attributes;

namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Stores registered task providers and resolves definitions.
/// </summary>
[RegisterSingleInstance]
internal sealed class TaskCatalog
{
    private readonly Dictionary<(string ProviderId, string DefinitionId, int Version), ITaskProvider> _map = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskCatalog" /> class.
    /// </summary>
    /// <param name="providers">The registered task providers.</param>
    public TaskCatalog(ITaskProvider[] providers)
    {
        foreach (var p in providers)
        {
            foreach (var d in p.GetDefinitions())
            {
                _map[(p.ProviderId, d.DefinitionId, d.Version)] = p;
            }
        }
    }

    /// <summary>
    ///     Tries to create a definition from a task record.
    /// </summary>
    /// <param name="record">The task record.</param>
    /// <param name="definition">The resolved definition.</param>
    /// <param name="error">The error message.</param>
    /// <returns>True if a definition was created; otherwise false.</returns>
    public bool TryCreate(TaskRecord record, out IManagedTaskDefinition? definition, out string? error)
    {
        if (!_map.TryGetValue((record.ProviderId, record.DefinitionId, record.DefinitionVersion), out var provider))
        {
            definition = null;
            error = $"No provider found for {record.ProviderId}/{record.DefinitionId}@{record.DefinitionVersion}";
            return false;
        }

        try
        {
            definition = provider.Create(new TaskDefinitionCreateContext(record.DefinitionId, record.DefinitionVersion,
                record.PayloadJson));
            error = null;
            return true;
        }
        catch (Exception ex)
        {
            definition = null;
            error = $"Provider '{record.ProviderId}' failed to create definition: {ex.Message}";
            return false;
        }
    }
}
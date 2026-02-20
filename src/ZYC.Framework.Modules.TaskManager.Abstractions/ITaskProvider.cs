namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Provides task definitions.
/// </summary>
public interface ITaskProvider
{
    /// <summary>
    ///     Gets the provider identifier (e.g. "default", "automation").
    /// </summary>
    string ProviderId { get; }

    /// <summary>
    ///     Gets the available task definitions.
    /// </summary>
    /// <returns>The definition descriptors.</returns>
    IEnumerable<TaskDefinitionDescriptor> GetDefinitions();

    /// <summary>
    ///     Creates a definition for new or restored tasks.
    /// </summary>
    /// <param name="createContext">The creation context.</param>
    /// <returns>The managed task definition.</returns>
    IManagedTaskDefinition Create(TaskDefinitionCreateContext createContext);
}
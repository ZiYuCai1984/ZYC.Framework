namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Provides context for creating a task definition.
/// </summary>
public class TaskDefinitionCreateContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TaskDefinitionCreateContext" /> class.
    /// </summary>
    /// <param name="definitionId">The definition identifier.</param>
    /// <param name="version">The definition version.</param>
    /// <param name="payloadJson">The payload JSON.</param>
    public TaskDefinitionCreateContext(string definitionId, int version, string payloadJson)
    {
        DefinitionId = definitionId;
        Version = version;
        PayloadJson = payloadJson;
    }

    /// <summary>
    ///     Gets the definition identifier.
    /// </summary>
    public string DefinitionId { get; }

    /// <summary>
    ///     Gets the definition version.
    /// </summary>
    public int Version { get; }

    /// <summary>
    ///     Gets the payload JSON.
    /// </summary>
    public string PayloadJson { get; }
}
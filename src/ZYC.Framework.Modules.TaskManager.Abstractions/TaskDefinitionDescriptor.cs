namespace ZYC.Framework.Modules.TaskManager.Abstractions;

/// <summary>
///     Describes a task definition.
/// </summary>
/// <param name="DefinitionType">The definition type.</param>
/// <param name="DefinitionId">The definition identifier.</param>
/// <param name="Version">The definition version.</param>
/// <param name="DisplayName">The display name.</param>
/// <param name="Description">The optional description.</param>
public sealed record TaskDefinitionDescriptor(
    Type DefinitionType,
    string DefinitionId,
    int Version,
    string DisplayName,
    string? Description = null);
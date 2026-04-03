namespace ZYC.Framework.Abstractions.Workspace;

/// <summary>
///     Represents a user-defined workspace layout preset, including its metadata,
///     preview thumbnail, and the workspace tree used to restore it.
/// </summary>
public class CustomWorkspaceLayout
{
    /// <summary>
    ///     Gets or sets the unique identifier of the saved layout.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     Gets or sets the display name of the saved layout.
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    ///     Gets or sets the preview icon payload of the saved layout.
    /// </summary>
    public string? Thumbnail { get; set; }

    /// <summary>
    ///     Gets or sets the workspace tree snapshot used to restore the layout.
    /// </summary>
    public WorkspaceNode WorkspaceNode { get; set; } = null!;
}

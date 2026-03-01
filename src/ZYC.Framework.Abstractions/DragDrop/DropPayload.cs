namespace ZYC.Framework.Abstractions.DragDrop;

/// <summary>
///     Represents the data package carried during a drag-and-drop operation.
/// </summary>
public sealed record DropPayload
{
    // A cached empty dictionary to avoid unnecessary allocations.
    private static readonly IReadOnlyDictionary<string, object?> EmptyExtras =
        new Dictionary<string, object?>();

    /// <summary>
    ///     DropPayload
    /// </summary>
    /// <param name="paths"></param>
    /// <param name="extras"></param>
    public DropPayload(string[] paths, IReadOnlyDictionary<string, object?>? extras = null)
    {
        Paths = paths;
        // Ensures Extras is never null for safer access.
        Extras = extras ?? EmptyExtras;
    }

    /// <summary>
    ///     Gets the collection of file paths or identifiers associated with the drop.
    /// </summary>
    public string[] Paths { get; }

    /// <summary>
    ///     Gets additional metadata or context-specific properties.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Extras { get; }
}
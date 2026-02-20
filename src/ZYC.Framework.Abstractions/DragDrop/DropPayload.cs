namespace ZYC.Framework.Abstractions.DragDrop;


public sealed record DropPayload
{
    private static readonly IReadOnlyDictionary<string, object?> EmptyExtras =
        new Dictionary<string, object?>();

    public DropPayload(string[] paths, IReadOnlyDictionary<string, object?>? extras = null)
    {
        Paths = paths;
        Extras = extras ?? EmptyExtras;
    }

    public string[] Paths { get; }

    public IReadOnlyDictionary<string, object?> Extras { get; }
}
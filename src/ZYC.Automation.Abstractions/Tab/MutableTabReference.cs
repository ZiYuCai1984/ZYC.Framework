namespace ZYC.Automation.Abstractions.Tab;

/// <summary>
///     A specialized version of <see cref="TabReference" /> that allows the <see cref="Uri" /> to be modified after
///     initialization.
/// </summary>
public sealed class MutableTabReference : TabReference
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MutableTabReference" /> class.
    /// </summary>
    /// <param name="uri">The initial address.</param>
    public MutableTabReference(Uri uri) : base(uri)
    {
    }

    /// <summary>
    ///     Gets or sets the URI of the tab.
    /// </summary>
    public new Uri Uri
    {
        get => base.Uri;
        set => base.Uri = value;
    }
}
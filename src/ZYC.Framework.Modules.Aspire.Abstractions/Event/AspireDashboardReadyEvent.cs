namespace ZYC.Framework.Modules.Aspire.Abstractions.Event;

/// <summary>
///     Event raised when the Aspire dashboard is ready.
/// </summary>
public sealed class AspireDashboardReadyEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AspireDashboardReadyEvent" /> class.
    /// </summary>
    /// <param name="uri">The dashboard URI.</param>
    public AspireDashboardReadyEvent(Uri uri)
    {
        Uri = uri;
    }

    /// <summary>
    ///     Gets the dashboard URI.
    /// </summary>
    public Uri Uri { get; }
}
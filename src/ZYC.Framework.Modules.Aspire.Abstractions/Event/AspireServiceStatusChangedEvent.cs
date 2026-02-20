namespace ZYC.Framework.Modules.Aspire.Abstractions.Event;

/// <summary>
///     Event raised when the Aspire service status changes.
/// </summary>
public class AspireServiceStatusChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AspireServiceStatusChangedEvent" /> class.
    /// </summary>
    /// <param name="aspireServiceStatus">The updated Aspire service status.</param>
    public AspireServiceStatusChangedEvent(AspireServiceStatus aspireServiceStatus)
    {
        AspireServiceStatus = aspireServiceStatus;
    }

    /// <summary>
    ///     Gets the updated Aspire service status.
    /// </summary>
    public AspireServiceStatus AspireServiceStatus { get; }
}
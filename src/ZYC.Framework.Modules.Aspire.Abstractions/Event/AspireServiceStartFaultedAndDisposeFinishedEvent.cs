namespace ZYC.Framework.Modules.Aspire.Abstractions.Event;

/// <summary>
///     Event raised when Aspire startup fails and disposal is complete.
/// </summary>
public class AspireServiceStartFaultedAndDisposeFinishedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AspireServiceStartFaultedAndDisposeFinishedEvent" /> class.
    /// </summary>
    /// <param name="exception">The exception that occurred during startup.</param>
    public AspireServiceStartFaultedAndDisposeFinishedEvent(Exception exception)
    {
        Exception = exception;
    }

    /// <summary>
    ///     Gets the exception that occurred during startup.
    /// </summary>
    public Exception Exception { get; }
}
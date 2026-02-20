namespace ZYC.Framework.Modules.Aspire.Abstractions;

/// <summary>
///     Represents the current status of the Aspire service.
/// </summary>
public class AspireServiceStatus
{
    private AspireServiceStatus()
    {
    }

    private AspireServiceStatus(
        ServiceStatus status,
        Uri? dashboardUri = null,
        Exception? exception = null)
    {
        DashboardUri = dashboardUri;
        Status = status;
        Exception = exception;
    }

    /// <summary>
    ///     Gets the dashboard URI when the service is available.
    /// </summary>
    public Uri? DashboardUri { get; }

    /// <summary>
    ///     Gets the service status value.
    /// </summary>
    public ServiceStatus Status { get; }

    /// <summary>
    ///     Gets the exception associated with a faulted status, if any.
    /// </summary>
    public Exception? Exception { get; }


    /// <summary>
    ///     Creates a stopped status.
    /// </summary>
    /// <param name="exception">Optional exception that caused the stop.</param>
    /// <returns>A stopped service status.</returns>
    public static AspireServiceStatus Stopped(Exception? exception = null)
    {
        return new AspireServiceStatus(ServiceStatus.Stopped, null, exception);
    }

    /// <summary>
    ///     Creates a stopping status.
    /// </summary>
    /// <param name="exception">Optional exception that caused the stop request.</param>
    /// <returns>A stopping service status.</returns>
    public static AspireServiceStatus Stopping(Exception? exception = null)
    {
        return new AspireServiceStatus(ServiceStatus.Stopping, null, exception);
    }

    /// <summary>
    ///     Creates a starting status.
    /// </summary>
    /// <param name="dashboardUri">The dashboard URI that will be used once running.</param>
    /// <returns>A starting service status.</returns>
    public static AspireServiceStatus Starting(Uri dashboardUri)
    {
        return new AspireServiceStatus(ServiceStatus.Starting);
    }

    /// <summary>
    ///     Creates a running status.
    /// </summary>
    /// <param name="dashboardUri">The active dashboard URI.</param>
    /// <returns>A running service status.</returns>
    public static AspireServiceStatus Running(Uri dashboardUri)
    {
        return new AspireServiceStatus(ServiceStatus.Running, dashboardUri);
    }
}
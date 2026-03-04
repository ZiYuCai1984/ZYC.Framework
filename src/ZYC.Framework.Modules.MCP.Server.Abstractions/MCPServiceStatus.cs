namespace ZYC.Framework.Modules.MCP.Server.Abstractions;

/// <summary>
///     Represents the runtime status of an MCP service.
/// </summary>
public class MCPServiceStatus
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MCPServiceStatus" /> class.
    /// </summary>
    /// <param name="serviceStatus">The current service status.</param>
    /// <param name="exception">
    ///     An optional exception associated with the status, typically representing
    ///     the cause of a failure or stop.
    /// </param>
    public MCPServiceStatus(ServiceStatus serviceStatus, Exception? exception = null)
    {
        ServiceStatus = serviceStatus;
        Exception = exception;
    }

    /// <summary>
    ///     Gets the current service status.
    /// </summary>
    public ServiceStatus ServiceStatus { get; }

    /// <summary>
    ///     Gets the exception associated with the service state, if any.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    ///     Creates a status indicating that the service has stopped.
    /// </summary>
    /// <param name="exception">
    ///     Optional exception describing the reason for the stop.
    /// </param>
    /// <returns>A <see cref="MCPServiceStatus" /> representing a stopped service.</returns>
    public static MCPServiceStatus Stopped(Exception? exception = null)
    {
        return new MCPServiceStatus(ServiceStatus.Stopped, exception);
    }

    /// <summary>
    ///     Creates a status indicating that the service is stopping.
    /// </summary>
    /// <param name="exception">
    ///     Optional exception describing the reason for the stop request.
    /// </param>
    /// <returns>A <see cref="MCPServiceStatus" /> representing a stopping service.</returns>
    public static MCPServiceStatus Stopping(Exception? exception = null)
    {
        return new MCPServiceStatus(ServiceStatus.Stopping, exception);
    }

    /// <summary>
    ///     Creates a status indicating that the service is starting.
    /// </summary>
    /// <returns>A <see cref="MCPServiceStatus" /> representing a starting service.</returns>
    public static MCPServiceStatus Starting()
    {
        return new MCPServiceStatus(ServiceStatus.Starting);
    }

    /// <summary>
    ///     Creates a status indicating that the service is running.
    /// </summary>
    /// <returns>A <see cref="MCPServiceStatus" /> representing a running service.</returns>
    public static MCPServiceStatus Running()
    {
        return new MCPServiceStatus(ServiceStatus.Running);
    }
}
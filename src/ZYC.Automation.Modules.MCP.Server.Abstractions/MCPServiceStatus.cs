namespace ZYC.Automation.Modules.MCP.Server.Abstractions;

public class MCPServiceStatus
{
    public MCPServiceStatus(ServiceStatus serviceStatus, Exception? exception = null)
    {
        ServiceStatus = serviceStatus;
        Exception = exception;
    }

    public ServiceStatus ServiceStatus { get; }

    public Exception? Exception { get; }

    /// <summary>
    ///     Creates a stopped status.
    /// </summary>
    /// <param name="exception">Optional exception that caused the stop.</param>
    /// <returns>A stopped service status.</returns>
    public static MCPServiceStatus Stopped(Exception? exception = null)
    {
        return new MCPServiceStatus(ServiceStatus.Stopped, exception);
    }

    /// <summary>
    ///     Creates a stopping status.
    /// </summary>
    /// <param name="exception">Optional exception that caused the stop request.</param>
    /// <returns>A stopping service status.</returns>
    public static MCPServiceStatus Stopping(Exception? exception = null)
    {
        return new MCPServiceStatus(ServiceStatus.Stopping, exception);
    }

    /// <summary>
    ///     Creates a starting status.
    /// </summary>
    /// <returns>A starting service status.</returns>
    public static MCPServiceStatus Starting()
    {
        return new MCPServiceStatus(ServiceStatus.Starting);
    }

    /// <summary>
    ///     Creates a running status.
    /// </summary>
    /// <returns>A running service status.</returns>
    public static MCPServiceStatus Running()
    {
        return new MCPServiceStatus(ServiceStatus.Running);
    }
}
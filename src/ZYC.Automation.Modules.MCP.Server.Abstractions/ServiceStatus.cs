namespace ZYC.Automation.Modules.MCP.Server.Abstractions;

/// <summary>
///     Represents the lifecycle state of the service.
/// </summary>
public enum ServiceStatus
{
    /// <summary>
    ///     The service is stopped.
    /// </summary>
    Stopped = 0,

    /// <summary>
    ///     The service is stopping.
    /// </summary>
    Stopping = 1,

    /// <summary>
    ///     The service is starting.
    /// </summary>
    Starting = 2,

    /// <summary>
    ///     The service is running.
    /// </summary>
    Running = 3
}
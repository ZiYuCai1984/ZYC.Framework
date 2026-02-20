namespace ZYC.Framework.Modules.MCP.Server.Abstractions;

/// <summary>
///     MCP Server Manager
/// </summary>
public interface IMCPServerManager
{
    /// <summary>
    ///     Starts the MCP server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StartServerAsync();

    /// <summary>
    ///     Stops the MCP server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StopServerAsync();

    /// <summary>
    ///     Gets a snapshot of the current service status.
    /// </summary>
    /// <returns>The current service status.</returns>
    MCPServiceStatus GetStatusSnapshot();
}
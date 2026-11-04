namespace ZYC.Framework.Modules.MCP.Server.Abstractions.Event;

/// <summary>
///     Event raised when the MCP service status changes.
/// </summary>
public class MCPServiceStatusChangedEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MCPServiceStatusChangedEvent" /> class.
    /// </summary>
    /// <param name="mcpServiceStatus">The updated MCP service status.</param>
    public MCPServiceStatusChangedEvent(MCPServiceStatus mcpServiceStatus)
    {
        MCPServiceStatus = mcpServiceStatus;
    }

    /// <summary>
    ///     Gets the updated MCP service status.
    /// </summary>
    public MCPServiceStatus MCPServiceStatus { get; }
}
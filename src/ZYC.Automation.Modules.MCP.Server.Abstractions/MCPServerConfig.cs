using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Automation.Modules.MCP.Server.Abstractions;

/// <summary>
///     Configuration settings for the MCP server.
/// </summary>
public class MCPServerConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the port number the MCP server listens on.
    ///     Default is 3004.
    /// </summary>
    public int Port { get; set; } = 3004;
}
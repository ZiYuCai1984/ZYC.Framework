using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.MCP.Server.Abstractions;

/// <summary>
///     Configuration settings for the MCP server.
/// </summary>
public class MCPServerConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the port number the MCP server listens on.
    /// </summary>
    public int Port { get; set; } = 3004;

    /// <summary>
    ///     Gets or sets a value indicating whether the MCP service should start automatically.
    /// </summary>
    public bool AutoStart { get; set; }

    /// <summary>
    ///     Gets or sets a regular expression matched against MCP tool names (case-insensitive).
    ///     Matching tools are not exposed. Empty means all tools are exposed.
    ///     Applied when the MCP server starts.
    /// </summary>
    /// <example>^SecretsManager_|^SettingsManager_Reset</example>
    public string ExcludedToolsPattern { get; set; } = string.Empty;
}
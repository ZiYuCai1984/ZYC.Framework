using ZYC.Automation.Abstractions.MCP;

namespace ZYC.Automation.Modules.Aspire.Abstractions;

/// <summary>
///     Manages the Aspire service lifecycle and tool acquisition.
/// </summary>
[ExposeToMCP]
public interface IAspireServiceManager
{
    /// <summary>
    ///     Downloads the Aspire tooling required by the module.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DownloadAspireToolsAsync();

    /// <summary>
    ///     Starts the Aspire server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StartServerAsync();

    /// <summary>
    ///     Stops the Aspire server.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task StopServerAsync();

    /// <summary>
    ///     Gets a snapshot of the current service status.
    /// </summary>
    /// <returns>The current service status.</returns>
    AspireServiceStatus GetStatusSnapshot();

    /// <summary>
    ///     Sets the source for resolving Aspire binaries.
    /// </summary>
    /// <param name="source">The selected binary source.</param>
    void SetAspireBinarySource(AspireBinarySource source);
}
using ZYC.Framework.Abstractions.MCP;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Manages NuGet-based modules and their installation lifecycle.
/// </summary>
[ExposeToMCP]
public interface INuGetModuleManager
{
    /// <summary>
    ///     Gets available NuGet modules.
    /// </summary>
    /// <returns>An array of available modules.</returns>
    Task<INuGetModule[]> GetModulesAsync();

    /// <summary>
    ///     Installs the specified NuGet module.
    /// </summary>
    /// <param name="module">The module to install.</param>
    Task InstallAsync(INuGetModule module);

    /// <summary>
    ///     Uninstalls the specified NuGet module.
    /// </summary>
    /// <param name="module">The module to uninstall.</param>
    Task UninstallAsync(INuGetModule module);

    /// <summary>
    ///     Gets the absolute or relative file path to the NuGet module's assets JSON file (typically 'project.assets.json').
    /// </summary>
    /// <returns>A string representing the full path to the module's asset configuration file.</returns>
    string GetNuGetModuleAssetsJsonPath();
}
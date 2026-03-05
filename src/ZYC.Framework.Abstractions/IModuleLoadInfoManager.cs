using ZYC.Framework.Abstractions.MCP;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Manages the loading status and error information for system modules.
/// </summary>
[ExposeToMCP]
public partial interface IModuleLoadInfoManager
{
    /// <summary>
    ///     Retrieves an array of all errors encountered during the module loading process.
    /// </summary>
    /// <returns>An array of <see cref="ModuleLoadErrorInfo" /> containing error details.</returns>
    ModuleLoadErrorInfo[] GetLoadErrorInfos();
}

public partial interface IModuleLoadInfoManager
{
    /// <summary>
    ///     Sets the collection of module load error information.
    /// </summary>
    /// <param name="moduleLoadErrorInfos">The array of error information to be stored.</param>
    /// <remarks>
    ///     This method is marked as internal and is intended to be used only within
    ///     the defining assembly to initialize or update error states.
    /// </remarks>
    [MCPIgnore]
    internal void SetLoadErrorInfos(ModuleLoadErrorInfo[] moduleLoadErrorInfos);

    /// <summary>
    ///     Display the page of module load error information.
    /// </summary>
    [MCPIgnore]
    void DisplayLoadErrorInfoPage();
}
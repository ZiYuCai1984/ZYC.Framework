using ZYC.CoreToolkit.Abstractions.Autofac;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Manages locally installed modules and their dependencies.
/// </summary>
public interface ILocalModuleManager
{
    /// <summary>
    ///     Gets all locally installed modules.
    /// </summary>
    /// <returns>An array of module information.</returns>
    IModuleInfo[] GetModules();

    /// <summary>
    ///     Gets the dependencies required by the specified module.
    /// </summary>
    /// <param name="moduleInfo">The module to inspect.</param>
    /// <returns>The modules that the specified module depends on.</returns>
    IModuleInfo[] GetDependency(IModuleInfo moduleInfo);

    /// <summary>
    ///     Gets modules that depend on the specified module.
    /// </summary>
    /// <param name="moduleInfo">The module to inspect.</param>
    /// <returns>The modules that depend on the specified module.</returns>
    IModuleInfo[] GetDependencyBy(IModuleInfo moduleInfo);

    /// <summary>
    ///     Disables the specified module.
    /// </summary>
    /// <param name="moduleInfo">The module to disable.</param>
    void DisableModule(IModuleInfo moduleInfo);

    /// <summary>
    ///     Enables the specified module.
    /// </summary>
    /// <param name="moduleInfo">The module to enable.</param>
    void EnableModule(IModuleInfo moduleInfo);

    /// <summary>
    ///     Deletes the specified module and its dependent modules recursively.
    /// </summary>
    /// <param name="moduleInfo">The module to delete.</param>
    void DeleteModuleRecursive(IModuleInfo moduleInfo);

    /// <summary>
    ///     Gets DLLs pending removal.
    /// </summary>
    /// <returns>DLL file names pending removal.</returns>
    string[] GetPendingRemoveDlls();

    /// <summary>
    ///     Determines whether a module is disabled.
    /// </summary>
    /// <param name="moduleInfo">The module to check.</param>
    /// <returns><c>true</c> if the module is disabled; otherwise, <c>false</c>.</returns>
    bool IsModuleDisabled(IModuleInfo moduleInfo)
    {
        return IsModuleDisabled(moduleInfo.ModuleAssemblyName);
    }

    /// <summary>
    ///     Determines whether a module is disabled by assembly name.
    /// </summary>
    /// <param name="moduleAssemblyName">The module assembly name.</param>
    /// <returns><c>true</c> if the module is disabled; otherwise, <c>false</c>.</returns>
    bool IsModuleDisabled(string moduleAssemblyName);

    /// <summary>
    ///     Determines whether a module is pending deletion.
    /// </summary>
    /// <param name="moduleInfo">The module to check.</param>
    /// <returns><c>true</c> if the module is pending deletion; otherwise, <c>false</c>.</returns>
    bool IsMoudlePendingDelete(IModuleInfo moduleInfo)
    {
        return IsMoudlePendingDelete(moduleInfo.ModuleAssemblyName);
    }

    /// <summary>
    ///     Determines whether a module is pending deletion by assembly name.
    /// </summary>
    /// <param name="moduleAssemblyName">The module assembly name.</param>
    /// <returns><c>true</c> if the module is pending deletion; otherwise, <c>false</c>.</returns>
    bool IsMoudlePendingDelete(string moduleAssemblyName);
}
using System.Diagnostics;
using System.Reflection;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides helper methods for resolving module names and DLL names.
/// </summary>
public static class ModuleNameTools
{
    /// <summary>
    ///     Gets the module name from an assembly name.
    /// </summary>
    /// <param name="assemblyName">The assembly name to inspect.</param>
    /// <returns>The module name, or null if the name does not match.</returns>
    public static string? GetModuleNameFromAssemblyName(string assemblyName)
    {
        var prefix = "ZYC.Framework.Modules.";
        var suffix = ".Abstractions";

        if (!assemblyName.Contains(prefix))
        {
            return null;
        }

        var moduleName = assemblyName.Replace(prefix, "")
            .Replace(suffix, "");
        return moduleName;
    }

    /// <summary>
    ///     Gets the module-qualified name for a type.
    /// </summary>
    /// <param name="type">The type to inspect.</param>
    /// <returns>The module-qualified name.</returns>
    public static string GetTypeModuleName(Type type)
    {
        var assemblyName = type.Assembly.GetName().Name!;
        var moduleName = GetModuleNameFromAssemblyName(assemblyName);

        if (moduleName == null)
        {
            return type.Name;
        }

        return $"{moduleName}.{type.Name}";
    }

    /// <summary>
    ///     Gets the abstractions DLL name for a module assembly.
    /// </summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The abstractions DLL name.</returns>
    public static string GetModuleAbstractionsDllName(Assembly assembly)
    {
        var name = assembly.GetName().Name;

        Debug.Assert(name != null);

        if (name.EndsWith(".Abstractions"))
        {
            return $"{name}.dll";
        }

        return $"{name}.Abstractions.dll";
    }

    /// <summary>
    ///     Gets the implementation DLL name for a module assembly.
    /// </summary>
    /// <param name="assembly">The assembly to inspect.</param>
    /// <returns>The module DLL name.</returns>
    public static string GetModuleDllName(Assembly assembly)
    {
        var name = assembly.GetName().Name;

        Debug.Assert(name != null);

        //!WARNING There are implementation vulnerabilities
        if (name.EndsWith(".Abstractions"))
        {
            return $"{name.Replace(".Abstractions", "")}.dll";
        }

        return $"{name}.dll";
    }

    /// <summary>
    ///     Converts an abstractions DLL name to the implementation DLL name.
    /// </summary>
    /// <param name="dllName">The DLL name to convert.</param>
    /// <returns>The module DLL name.</returns>
    public static string GetModuleDllName(string dllName)
    {
        //!WARNING There are implementation vulnerabilities
        if (dllName.EndsWith(".Abstractions.dll"))
        {
            return dllName.Replace(".Abstractions.dll", ".dll");
        }

        return dllName;
    }
}
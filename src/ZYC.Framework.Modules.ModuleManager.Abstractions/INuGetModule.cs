using ZYC.CoreToolkit.Abstractions.Autofac;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions;

/// <summary>
///     Describes a NuGet module and its metadata.
/// </summary>
public interface INuGetModule : IModuleInfo
{
    /// <summary>
    ///     Gets the NuGet package identifier.
    /// </summary>
    string PackageId { get; }

    /// <summary>
    ///     Gets the module version.
    /// </summary>
    string Version { get; }

    /// <summary>
    ///     Gets a value indicating whether the module is installed.
    /// </summary>
    bool Installed { get; }

    /// <summary>
    ///     Gets the patch notes for the module.
    /// </summary>
    string PatchNote { get; }
}
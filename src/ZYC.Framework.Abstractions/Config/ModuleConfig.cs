using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for managing application modules and external assemblies.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
public class ModuleConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a list of assembly names that should be explicitly disabled or ignored.
    /// </summary>
    public string[] DisabledAssemblyNames { get; set; } = [];

    /// <summary>
    ///     Gets or sets a list of additional assembly names to be loaded into the application.
    /// </summary>
    public string[] AdditionalAssemblyNames { get; set; } = [];
}
using ZYC.CoreToolkit.Abstractions.Autofac;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions.Event;

/// <summary>
///     Represents a request to enable a module.
/// </summary>
public sealed class EnableModuleEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EnableModuleEvent" /> class.
    /// </summary>
    /// <param name="moduleInfo">The module to enable.</param>
    public EnableModuleEvent(IModuleInfo moduleInfo)
    {
        ModuleInfo = moduleInfo;
    }

    /// <summary>
    ///     Gets the module information to enable.
    /// </summary>
    public IModuleInfo ModuleInfo { get; }
}
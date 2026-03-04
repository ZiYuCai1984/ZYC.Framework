using ZYC.CoreToolkit.Abstractions.Autofac;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions.Event;

/// <summary>
///     Represents a request to disable a module.
/// </summary>
public sealed class DisableModuleEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DisableModuleEvent" /> class.
    /// </summary>
    /// <param name="moduleInfo">The module to disable.</param>
    public DisableModuleEvent(IModuleInfo moduleInfo)
    {
        ModuleInfo = moduleInfo;
    }

    /// <summary>
    ///     Gets the module information to disable.
    /// </summary>
    public IModuleInfo ModuleInfo { get; }
}
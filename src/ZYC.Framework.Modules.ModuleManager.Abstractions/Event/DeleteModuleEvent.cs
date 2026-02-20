using ZYC.CoreToolkit.Abstractions.Autofac;

namespace ZYC.Framework.Modules.ModuleManager.Abstractions.Event;

/// <summary>
///     Represents a request to delete a module.
/// </summary>
public class DeleteModuleEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeleteModuleEvent" /> class.
    /// </summary>
    /// <param name="moduleInfo">The module to delete.</param>
    public DeleteModuleEvent(IModuleInfo moduleInfo)
    {
        ModuleInfo = moduleInfo;
    }

    /// <summary>
    ///     Gets the module information to delete.
    /// </summary>
    public IModuleInfo ModuleInfo { get; }
}
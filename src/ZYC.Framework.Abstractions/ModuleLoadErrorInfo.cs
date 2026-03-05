using ZYC.CoreToolkit.Abstractions.Autofac;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Represents detailed information about an error that occurred during the module loading process.
/// </summary>
public class ModuleLoadErrorInfo
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ModuleLoadErrorInfo" /> class.
    /// </summary>
    /// <param name="moduleInfo">The metadata or information of the module that failed to load.</param>
    /// <param name="exception">The actual exception thrown during the loading process.</param>
    /// <param name="function">The name of the specific function or stage where the error occurred (optional).</param>
    public ModuleLoadErrorInfo(IModuleInfo moduleInfo, Exception exception, string function = "")
    {
        ModuleInfo = moduleInfo;
        Exception = exception;
        Function = function;
    }

    /// <summary>
    ///     Gets the information of the module associated with the error.
    /// </summary>
    public IModuleInfo ModuleInfo { get; }

    /// <summary>
    ///     Gets the exception details associated with the failure.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    ///     Gets the name of the function or member where the failure originated.
    /// </summary>
    public string Function { get; }
}
using System.Windows.Input;

namespace ZYC.Framework.Modules.Update.Abstractions.Commands;

/// <summary>
///     Represents a command that triggers an update check operation.
/// </summary>
public interface ICheckUpdateCommand : ICommand
{
    /// <summary>
    ///     Executes the update check command without a parameter.
    /// </summary>
    /// <remarks>
    ///     This method is a convenience wrapper around <see cref="ICommand.Execute(object?)" />
    ///     and calls it with a <c>null</c> parameter.
    /// </remarks>
    void Execute()
    {
        Execute(null);
    }
}
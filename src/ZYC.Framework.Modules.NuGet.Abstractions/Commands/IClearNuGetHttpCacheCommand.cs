using System.Windows.Input;

namespace ZYC.Framework.Modules.NuGet.Abstractions.Commands;

/// <summary>
///     Defines a command interface for clearing the NuGet HTTP cache.
/// </summary>
public interface IClearNuGetHttpCacheCommand : ICommand
{
    /// <summary>
    ///     Executes the cache clearing logic with a default null parameter.
    ///     This provides a simplified entry point for the command.
    /// </summary>
    void Execute()
    {
        // Forwards the call to the parameterized Execute method, 
        // passing null as the default argument.
        Execute(null);
    }
}
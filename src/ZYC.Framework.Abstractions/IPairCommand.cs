using System.Windows.Input;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Represents a command that participates in paired command behaviors.
/// </summary>
public interface IPairCommand
{
    /// <summary>
    ///     Gets the command instance.
    /// </summary>
    ICommand Self { get; }

    /// <summary>
    ///     Raises notifications that paired commands have changed.
    /// </summary>
    void RaisePairCommandsChanged();
}
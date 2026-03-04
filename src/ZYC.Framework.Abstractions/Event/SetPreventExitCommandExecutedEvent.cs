namespace ZYC.Framework.Abstractions.Event;

/// <summary>
///     Event raised when a command to prevent or allow application exit has been executed.
///     Typically used to toggle the "Close to Tray" or "Confirm Before Exit" behavior.
/// </summary>
public sealed class SetPreventExitCommandExecutedEvent
{
}
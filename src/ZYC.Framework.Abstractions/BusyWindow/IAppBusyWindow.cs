namespace ZYC.Framework.Abstractions.BusyWindow;

/// <summary>
///     Provides a service to display a "Busy" or "Loading" window to the user,
///     typically used to block UI interaction during long-running background tasks.
/// </summary>
public interface IAppBusyWindow
{
    /// <summary>
    ///     Enqueues a new busy state and returns a handler to control the busy window's UI.
    ///     If a window is already visible, it may increment a reference counter or update the current view.
    /// </summary>
    /// <returns>A <see cref="IBusyWindowHandler" /> to update or close the busy state.</returns>
    IBusyWindowHandler Enqueue();
}
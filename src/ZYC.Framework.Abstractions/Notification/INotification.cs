namespace ZYC.Framework.Abstractions.Notification;

/// <summary>
///     Defines the base contract for all notification types in the system.
///     Provides control over the notification's visibility and lifecycle.
/// </summary>
public interface INotification
{
    /// <summary>
    ///     Occurs when the notification is closed, either by user action or timeout.
    /// </summary>
    event EventHandler Closed;

    /// <summary>
    ///     Retrieves the current visibility state or the UI object responsible for visibility.
    /// </summary>
    /// <returns>An object representing the visibility status (e.g., a Visibility enum or a View element).</returns>
    object GetVisibility();
}
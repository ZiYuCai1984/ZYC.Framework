namespace ZYC.Framework.Abstractions.Notification;

public interface INotification
{
    event EventHandler Closed;

    object GetVisibility();
}
namespace ZYC.Framework.Abstractions.BusyWindow;

public interface IAppBusyWindow
{
    IBusyWindowHandler Enqueue();
}
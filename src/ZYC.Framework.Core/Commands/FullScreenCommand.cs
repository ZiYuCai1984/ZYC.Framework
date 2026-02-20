using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class FullScreenCommand : CommandBase
{
    public FullScreenCommand(IMainWindow mainWindow, IEventAggregator eventAggregator)
    {
        MainWindow = mainWindow;
        EventAggregator = eventAggregator;
    }

    private IMainWindow MainWindow { get; }

    private IEventAggregator EventAggregator { get; }

    protected override void InternalExecute(object? parameter)
    {
        var state = MainWindow.GetWindowState();

        if (state == WindowState.Maximized)
        {
            MainWindow.SetWindowState(WindowState.Normal);
        }
        else if (state == WindowState.Normal)
        {
            MainWindow.SetWindowState(WindowState.Maximized);
        }

        EventAggregator.Publish(new FullScreennCommandExecutedEvent());
    }
}
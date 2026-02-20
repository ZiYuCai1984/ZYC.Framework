using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class SetTopmostCommand : CommandBase
{
    public SetTopmostCommand(
        DesktopWindowState desktopWindowState,
        IMainWindow mainWindow,
        IEventAggregator eventAggregator)
    {
        DesktopWindowState = desktopWindowState;
        MainWindow = mainWindow;
        EventAggregator = eventAggregator;
    }

    private DesktopWindowState DesktopWindowState { get; }

    private IMainWindow MainWindow { get; }

    private IEventAggregator EventAggregator { get; }

    protected override void InternalExecute(object? parameter)
    {
        MainWindow.SetTopmost(!DesktopWindowState.Topmost);
        EventAggregator.Publish(new SetTopmostCommandExecutedEvent());
    }
}
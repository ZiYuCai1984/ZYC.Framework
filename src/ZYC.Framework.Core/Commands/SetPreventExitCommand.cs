using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Event;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class SetPreventExitCommand : CommandBase
{
    public SetPreventExitCommand(
        DesktopWindowState desktopWindowState,
        IEventAggregator eventAggregator)
    {
        DesktopWindowState = desktopWindowState;
        EventAggregator = eventAggregator;
    }

    private DesktopWindowState DesktopWindowState { get; }

    private IEventAggregator EventAggregator { get; }

    protected override void InternalExecute(object? parameter)
    {
        DesktopWindowState.IsPreventExit = !DesktopWindowState.IsPreventExit;
        EventAggregator.Publish(new SetPreventExitCommandExecutedEvent());
    }
}
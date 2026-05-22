using Autofac;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.State;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class AttachDebuggerCommand : CommandBase
{
    public AttachDebuggerCommand(ILifetimeScope lifetimeScope, DesktopWindowState desktopWindowState)
    {
        LifetimeScope = lifetimeScope;
        DesktopWindowState = desktopWindowState;
    }

    private ILifetimeScope LifetimeScope { get; }

    private DesktopWindowState DesktopWindowState { get; }

    protected override void InternalExecute(object? parameter)
    {
        if (DesktopWindowState.Topmost)
        {
            var setTopmostCommand = LifetimeScope.Resolve<SetTopmostCommand>();
            setTopmostCommand.Execute(null);

            DebuggerTools.Break();

            setTopmostCommand.Execute(null);
        }
        else
        {
            DebuggerTools.Break();
        }
    }
}
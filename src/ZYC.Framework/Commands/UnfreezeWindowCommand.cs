using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class UnfreezeWindowCommand : PairCommandBase<FreezeWindowCommand, UnfreezeWindowCommand>
{
    public UnfreezeWindowCommand(ILifetimeScope lifetimeScope, DesktopWindowState desktopWindowState) : base(
        lifetimeScope)
    {
        DesktopWindowState = desktopWindowState;
    }

    private DesktopWindowState DesktopWindowState { get; }

    protected override void InternalExecute(object? parameter)
    {
        LifetimeScope.Resolve<IMainWindow>().SetIsFrozen(false);
    }

    public override bool CanExecute(object? parameter)
    {
        return DesktopWindowState.IsFrozen;
    }
}
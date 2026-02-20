using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Core.Commands;

namespace ZYC.Framework.Commands;

[RegisterSingleInstance]
internal class FreezeWindowCommand : PairCommandBase<FreezeWindowCommand, UnfreezeWindowCommand>
{
    public FreezeWindowCommand(
        ILifetimeScope lifetimeScope, DesktopWindowState desktopWindowState) : base(
        lifetimeScope)
    {
        DesktopWindowState = desktopWindowState;
    }

    private DesktopWindowState DesktopWindowState { get; }

    protected override void InternalExecute(object? parameter)
    {
        LifetimeScope.Resolve<IMainWindow>().SetIsFrozen(true);
    }

    public override bool CanExecute(object? parameter)
    {
        return !DesktopWindowState.IsFrozen;
    }
}
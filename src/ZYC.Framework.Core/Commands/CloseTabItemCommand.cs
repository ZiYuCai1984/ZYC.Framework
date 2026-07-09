using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions.State;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Core.Commands;

[RegisterSingleInstance]
public class CloseTabItemCommand : CommandBase
{
    public CloseTabItemCommand(
        ITabManager tabManager,
        TabItemLockState tabItemLockState)
    {
        TabManager = tabManager;
        TabItemLockState = tabItemLockState;


        tabItemLockState.ObserveAnyChange().Subscribe(_ =>
        {
            RaiseCanExecuteChanged();
        });
    }

    private ITabManager TabManager { get; }

    private TabItemLockState TabItemLockState { get; }

    public override bool CanExecute(object? parameter)
    {
        if (parameter is not ITabItemInstance instance)
        {
            return false;
        }

        return !TabItemLockState.TabItems.Contains(instance.TabReference);
    }

    protected override void InternalExecute(object? parameter)
    {
        if (parameter is not ITabItemInstance instance)
        {
            return;
        }

        _ = TabManager.CloseAsync(instance);
    }
}

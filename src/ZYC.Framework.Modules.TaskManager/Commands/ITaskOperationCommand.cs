using Autofac;

namespace ZYC.Framework.Modules.TaskManager.Commands;

internal interface ITaskOperationCommand
{
    void RaiseCanExecuteChanged();
}

internal static class ITaskOperationCommandEx
{
    public static void RaiseTaskOperationCommandsCanExecuteChanged(this ILifetimeScope lifetimeScope)
    {
        var commands = lifetimeScope.Resolve<ITaskOperationCommand[]>();
        foreach (var c in commands)
        {
            c.RaiseCanExecuteChanged();
        }
    }
}
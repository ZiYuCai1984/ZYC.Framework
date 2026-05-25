using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ModuleManager.UI;

namespace ZYC.Framework.Modules.ModuleManager.Commands;

[RegisterSingleInstance]
internal class RefreshNuGetModuleCommand : AsyncCommandBase<NuGetModuleManagerView>
{
    public RefreshNuGetModuleCommand(NuGetModuleOperationCoordinator operationCoordinator)
    {
        OperationCoordinator = operationCoordinator;
        OperationCoordinator.IsExecutingChanged += (_, _) => RaiseCanExecuteChanged();
    }

    private NuGetModuleOperationCoordinator OperationCoordinator { get; }

    protected override async Task InternalExecuteAsync(NuGetModuleManagerView view)
    {
        if (OperationCoordinator.IsExecuting)
        {
            return;
        }

        await OperationCoordinator.TryRunAsync(view.RefreshNuGetModulesAsync);
    }

    protected override bool InternalCanExecute(NuGetModuleManagerView? parameter)
    {
        return parameter != null
               && !IsExecuting
               && !OperationCoordinator.IsExecuting;
    }
}

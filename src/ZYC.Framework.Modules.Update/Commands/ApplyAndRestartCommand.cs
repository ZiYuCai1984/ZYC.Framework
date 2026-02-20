using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Update.Abstractions;

namespace ZYC.Framework.Modules.Update.Commands;

[RegisterSingleInstance]
internal class ApplyAndRestartCommand : AsyncCommandBase
{
    public ApplyAndRestartCommand(
        RestartCommand restartCommand,
        IUpdateManager updateManager)
    {
        RestartCommand = restartCommand;
        UpdateManager = updateManager;
    }

    private RestartCommand RestartCommand { get; }

    private IUpdateManager UpdateManager { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        var product = (NewProduct)parameter!;

        await UpdateManager.ApplyProductAsync(product);
        RestartCommand.Execute(null);
    }


    public override bool CanExecute(object? parameter)
    {
        return !IsExecuting;
    }
}
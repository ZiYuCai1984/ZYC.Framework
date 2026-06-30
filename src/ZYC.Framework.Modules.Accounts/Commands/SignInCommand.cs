using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.Accounts.Abstractions;

namespace ZYC.Framework.Modules.Accounts.Commands;

[RegisterSingleInstance]
internal class SignInCommand : AsyncCommandBase<string>
{
    public SignInCommand(
        ILogger<SignInCommand> logger,
        IAccountManager accountManager,
        IToastManager toastManager)
    {
        Logger = logger;
        AccountManager = accountManager;
        ToastManager = toastManager;
    }

    private ILogger<SignInCommand> Logger { get; }

    private IAccountManager AccountManager { get; }

    private IToastManager ToastManager { get; }

    protected override async Task InternalExecuteAsync(string id)
    {
        try
        {
            await AccountManager.SignInAsync(id, CancellationToken.None);
        }
        catch (Exception ex)
        {
            ToastManager.PromptException(ex);
            Logger.Error(ex);
        }
    }

    public override bool CanExecute(object? parameter)
    {
        return AccountManager.GetCurrentSession() == null;
    }
}
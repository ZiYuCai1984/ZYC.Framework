using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ChromeExtensions.UI;

namespace ZYC.Framework.Modules.ChromeExtensions.Commands;

internal class InstallDetectedExtensionCommand : AsyncCommandBase
{
    public InstallDetectedExtensionCommand(ChromeWebStoreBrowserView view)
    {
        View = view;
    }

    private ChromeWebStoreBrowserView View { get; }

    public override bool CanExecute(object? parameter)
    {
        return View.CanInstallDetectedExtension;
    }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        await View.InstallDetectedExtensionAsync();
    }
}
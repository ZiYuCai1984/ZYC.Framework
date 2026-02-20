using Autofac;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.Commands;

[RegisterSingleInstance]
internal class
    UninstallNuGetModuleCommand : AsyncPairCommandBase<InstallNuGetModuleCommand, UninstallNuGetModuleCommand>
{
    public UninstallNuGetModuleCommand(
        IBannerManager bannerManager,
        IToastManager toastManager,
        IAppLogger<InstallNuGetModuleCommand> logger,
        ILifetimeScope lifetimeScope,
        INuGetModuleManager nuGetModuleManager,
        NuGetModuleState nuGetModuleState) : base(lifetimeScope)
    {
        BannerManager = bannerManager;
        ToastManager = toastManager;
        Logger = logger;
        NuGetModuleManager = nuGetModuleManager;
        NuGetModuleState = nuGetModuleState;
    }

    private IBannerManager BannerManager { get; }
    private IToastManager ToastManager { get; }
    private IAppLogger<InstallNuGetModuleCommand> Logger { get; }
    private INuGetModuleManager NuGetModuleManager { get; }

    private NuGetModuleState NuGetModuleState { get; }

    protected override async Task InternalExecuteAsync(object? parameter)
    {
        if (parameter == null)
        {
            return;
        }

        try
        {
            await NuGetModuleManager.UninstallAsync((INuGetModule)parameter);
            BannerManager.PromptRestart();
        }
        catch (Exception e)
        {
            Logger.Error(e);
            ToastManager.PromptException(e);
        }
    }

    public override bool CanExecute(object? parameter)
    {
        if (parameter == null)
        {
            return false;
        }

        var module = (INuGetModule)parameter;

        return !IsExecuting
               && NuGetModuleState.InstalledModules.Any(t =>
                   t.PackageId == module.PackageId && t.Version == module.Version);
    }
}
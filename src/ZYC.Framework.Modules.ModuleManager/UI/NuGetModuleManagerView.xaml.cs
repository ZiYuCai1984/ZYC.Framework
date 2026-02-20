using System.Collections.ObjectModel;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Localizations;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.Framework.Modules.ModuleManager.Commands;
using ZYC.Framework.Modules.NuGet.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.UI;

[Register]
internal sealed partial class NuGetModuleManagerView
{
    public NuGetModuleManagerView(
        NuGetConfig nugetConfig,
        IAppBusyWindow appBusyWindow,
        IToastManager toastManager,
        IAppLogger<NuGetModuleManagerView> logger,
        RefreshNuGetModuleCommand refreshNuGetModuleCommand,
        INuGetModuleManager nuGetModuleManager,
        INuGetManager nuGetManager)
    {
        NugetConfig = nugetConfig;
        AppBusyWindow = appBusyWindow;
        ToastManager = toastManager;
        Logger = logger;
        RefreshNuGetModuleCommand = refreshNuGetModuleCommand;
        NuGetModuleManager = nuGetModuleManager;
        NuGetManager = nuGetManager;
    }

    private NuGetConfig NugetConfig { get; }
    private IAppBusyWindow AppBusyWindow { get; }
    private IToastManager ToastManager { get; }
    private IAppLogger<NuGetModuleManagerView> Logger { get; }
    private RefreshNuGetModuleCommand RefreshNuGetModuleCommand { get; }

    private INuGetModuleManager NuGetModuleManager { get; }

    private INuGetManager NuGetManager { get; }

    public ObservableCollection<INuGetModule> NuGetModules { get; } = new();

    public int ModulesCount => NuGetModules.Count;

    protected override async void InternalOnLoaded()
    {
        base.InternalOnLoaded();
        await RefreshNuGetModuleCommand.ExecuteAsync(this);
    }

    public async Task RefreshNuGetModulesAsync()
    {
        var handler = AppBusyWindow.Enqueue();

        handler.Title = $"{L.Translate("Fetching NuGet packages from")} {NugetConfig.Source}";
        try
        {
            var modules = await NuGetModuleManager.GetModulesAsync();
            NuGetModules.Clear();
            foreach (var module in modules)
            {
                NuGetModules.Add(module);
            }

            OnPropertyChanged(nameof(ModulesCount));
        }
        catch (Exception e)
        {
            ToastManager.PromptException(e);
            Logger.Error(e);
        }
        finally
        {
            handler.Close();
        }
    }
}
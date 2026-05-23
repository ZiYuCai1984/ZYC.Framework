using System.Collections.ObjectModel;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Localizations;
using ZYC.Framework.Modules.ModuleManager;
using ZYC.Framework.Modules.ModuleManager.Abstractions;
using ZYC.Framework.Modules.ModuleManager.Commands;
using ZYC.Framework.Modules.NuGet.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.UI;

[Register]
internal sealed partial class NuGetModuleManagerView
{
    private INuGetModule? _selectedNuGetModule;

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

    private HashSet<string> LoadedPatchNoteKeys { get; } = new(StringComparer.OrdinalIgnoreCase);

    public ObservableCollection<INuGetModule> NuGetModules { get; } = new();

    public int ModulesCount => NuGetModules.Count;

    public INuGetModule? SelectedNuGetModule
    {
        get => _selectedNuGetModule;
        set
        {
            if (ReferenceEquals(_selectedNuGetModule, value))
            {
                return;
            }

            _selectedNuGetModule = value;
            OnPropertyChanged();

            _ = LoadPatchNoteAsync(value);
        }
    }

    protected override async void InternalOnLoaded()
    {
        base.InternalOnLoaded();
        await RefreshNuGetModuleCommand.ExecuteAsync(this);
    }

    public async Task RefreshNuGetModulesAsync()
    {
        var handler = AppBusyWindow.Enqueue();
        var selectedPackageId = SelectedNuGetModule?.PackageId;
        var selectedVersion = SelectedNuGetModule?.Version;

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
            SelectedNuGetModule = NuGetModules.FirstOrDefault(module =>
                                    string.Equals(
                                        module.PackageId,
                                        selectedPackageId,
                                        StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(
                                        module.Version,
                                        selectedVersion,
                                        StringComparison.OrdinalIgnoreCase))
                                ?? NuGetModules.FirstOrDefault(module =>
                                    string.Equals(
                                        module.PackageId,
                                        selectedPackageId,
                                        StringComparison.OrdinalIgnoreCase))
                                ?? NuGetModules.FirstOrDefault();
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

    private async Task LoadPatchNoteAsync(INuGetModule? module)
    {
        if (module is not NuGetModule nugetModule)
        {
            return;
        }

        var key = $"{module.PackageId}|{module.Version}";
        if (!LoadedPatchNoteKeys.Add(key))
        {
            return;
        }

        try
        {
            var patchNote = await NuGetManager.FetchReleaseNotesAsync(
                module.PackageId,
                module.Version);

            nugetModule.PatchNote = patchNote ?? string.Empty;
        }
        catch (Exception e)
        {
            LoadedPatchNoteKeys.Remove(key);
            Logger.Error(e);
        }
    }
}

using System.ComponentModel;
using System.Runtime.CompilerServices;
using ZYC.CoreToolkit.Abstractions.Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager;

internal class NuGetModule : INuGetModule, INotifyPropertyChanged
{
    private string? _installedVersion;
    private string _patchNote = "";

    public NuGetModule(string packageId, string version, string description, string? installedVersion)
    {
        PackageId = packageId;
        Version = version;
        Description = description;
        InstalledVersion = installedVersion;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Icon => Base64IconResources.NuGet;

    public string Description { get; }

    public string ModulePath => string.Empty;

    public string ModuleAssemblyName => PackageId;

    public string[] ReferenceAssemblyNames { get; } = [];

    public IModuleInfo[] DependencyOn { get; } = [];

    public IModuleInfo[] DependencyBy { get; } = [];

    public string PackageId { get; }

    public string Version { get; }

    public string? InstalledVersion
    {
        get => _installedVersion;
        internal set
        {
            if (_installedVersion == value)
            {
                return;
            }

            _installedVersion = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(Installed));
            OnPropertyChanged(nameof(InstalledVersionText));
            OnPropertyChanged(nameof(InstalledStatusText));
            OnPropertyChanged(nameof(InstallActionText));
            OnPropertyChanged(nameof(HasAvailableUpdate));
        }
    }

    public bool Installed => !string.IsNullOrWhiteSpace(InstalledVersion);

    public string InstalledVersionText => InstalledVersion ?? "Not installed";

    public bool HasAvailableUpdate => Installed
                                      && !string.Equals(
                                          InstalledVersion,
                                          Version,
                                          StringComparison.OrdinalIgnoreCase);

    public string InstalledStatusText => Installed
        ? $"Installed {InstalledVersion}"
        : "Not installed";

    public string InstallActionText
    {
        get
        {
            if (!Installed)
            {
                return "Install";
            }

            return HasAvailableUpdate ? "Update" : "Installed";
        }
    }

    public string PatchNote
    {
        get => _patchNote;
        internal set
        {
            _patchNote = value;
            OnPropertyChanged();
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

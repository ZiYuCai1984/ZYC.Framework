using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Extensions.Logging;
using NuGet.Versioning;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Banner;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.ModuleManager.Abstractions;

namespace ZYC.Framework.Modules.ModuleManager.UI;

[Register]
internal partial class ManualInstallNuGetModuleDialog : INotifyPropertyChanged
{
    private static readonly Regex PackageIdRegex = new(
        "^[A-Za-z0-9._-]{1,100}$",
        RegexOptions.CultureInvariant);

    private bool _isBusy;
    private string _packageId = "";
    private string _version = "";

    public ManualInstallNuGetModuleDialog(
        INuGetModuleManager nuGetModuleManager,
        IBannerManager bannerManager,
        IToastManager toastManager,
        ILogger<ManualInstallNuGetModuleDialog> logger)
    {
        NuGetModuleManager = nuGetModuleManager;
        BannerManager = bannerManager;
        ToastManager = toastManager;
        Logger = logger;

        DataContext = this;
        InitializeComponent();
    }

    private INuGetModuleManager NuGetModuleManager { get; }

    private IBannerManager BannerManager { get; }

    private IToastManager ToastManager { get; }

    private ILogger<ManualInstallNuGetModuleDialog> Logger { get; }

    public string PackageId
    {
        get => _packageId;
        set
        {
            if (_packageId == value)
            {
                return;
            }

            _packageId = value;
            OnPropertyChanged();
        }
    }

    public string Version
    {
        get => _version;
        set
        {
            if (_version == value)
            {
                return;
            }

            _version = value;
            OnPropertyChanged();
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsNotBusy));
        }
    }

    public bool IsNotBusy => !IsBusy;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnDialogLoaded(object sender, RoutedEventArgs e)
    {
        PackageIdTextBox.Focus();
    }

    private async void OnInstallButtonClick(object sender, RoutedEventArgs e)
    {
        if (IsBusy)
        {
            return;
        }

        var packageId = PackageId.Trim();
        var versionText = Version.Trim();
        if (!TryValidate(packageId, versionText, out var version))
        {
            return;
        }

        IsBusy = true;
        var succeeded = false;
        try
        {
            await NuGetModuleManager.InstallAsync(
                new NuGetModule(
                    packageId,
                    version.ToNormalizedString(),
                    "",
                    null));
            BannerManager.PromptRestart();
            succeeded = true;
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
            ToastManager.PromptException(exception);
        }
        finally
        {
            IsBusy = false;
        }

        if (!succeeded)
        {
            return;
        }

        Close();
    }

    private bool TryValidate(
        string packageId,
        string versionText,
        out NuGetVersion version)
    {
        if (!PackageIdRegex.IsMatch(packageId))
        {
            ToastManager.PromptMessage(
                ToastMessage.Warn("A valid package ID is required.", false));
            version = null!;
            return false;
        }

        if (!NuGetVersion.TryParse(versionText, out var parsedVersion)
            || parsedVersion == null)
        {
            ToastManager.PromptMessage(
                ToastMessage.Warn("A valid package version is required.", false));
            version = null!;
            return false;
        }

        version = parsedVersion;
        return true;
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

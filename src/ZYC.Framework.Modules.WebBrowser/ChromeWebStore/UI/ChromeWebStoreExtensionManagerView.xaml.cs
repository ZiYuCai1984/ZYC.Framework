using System.Collections.ObjectModel;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.WebBrowser.Abstractions.ChromeWebStore;

namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore.UI;

[Register]
internal partial class ChromeWebStoreExtensionManagerView
{
    private ChromeWebStoreExtensionPackageMetadata? _currentMetadata;
    private string _currentStoreExtensionId = "";
    private string _extensionIdText = "";
    private bool _isBusy;
    private ChromeWebStoreInstalledExtension? _selectedInstalledExtension;
    private string _statusText = "";

    public ChromeWebStoreExtensionManagerView(
        IAppBusyWindow appBusyWindow,
        IToastManager toastManager,
        IAppLogger<ChromeWebStoreExtensionManagerView> logger,
        IChromeWebStoreExtensionPackageMetadataProvider packageMetadataProvider,
        IChromeWebStoreExtensionPackageManager packageManager,
        ChromeWebStoreBrowserView storeBrowserView)
    {
        AppBusyWindow = appBusyWindow;
        ToastManager = toastManager;
        Logger = logger;
        PackageMetadataProvider = packageMetadataProvider;
        PackageManager = packageManager;
        StoreBrowserView = storeBrowserView;

        StoreBrowserView.CurrentExtensionIdChanged += OnStoreBrowserCurrentExtensionIdChanged;
        StoreBrowserHost.Content = StoreBrowserView;
    }

    private IAppBusyWindow AppBusyWindow { get; }

    private IToastManager ToastManager { get; }

    private IAppLogger<ChromeWebStoreExtensionManagerView> Logger { get; }

    private IChromeWebStoreExtensionPackageMetadataProvider PackageMetadataProvider { get; }

    private IChromeWebStoreExtensionPackageManager PackageManager { get; }

    private ChromeWebStoreBrowserView StoreBrowserView { get; }

    public ObservableCollection<ChromeWebStoreInstalledExtension> InstalledExtensions { get; } = new();

    public int InstalledExtensionsCount => InstalledExtensions.Count;

    public string ExtensionIdText
    {
        get => _extensionIdText;
        set
        {
            if (_extensionIdText == value)
            {
                return;
            }

            _extensionIdText = value;
            CurrentMetadata = null;
            OnPropertyChanged();
            RaiseOperationStateChanged();
        }
    }

    public ChromeWebStoreExtensionPackageMetadata? CurrentMetadata
    {
        get => _currentMetadata;
        private set
        {
            if (ReferenceEquals(_currentMetadata, value))
            {
                return;
            }

            _currentMetadata = value;
            OnPropertyChanged();
            RaiseOperationStateChanged();
        }
    }

    public ChromeWebStoreInstalledExtension? SelectedInstalledExtension
    {
        get => _selectedInstalledExtension;
        set
        {
            if (ReferenceEquals(_selectedInstalledExtension, value))
            {
                return;
            }

            _selectedInstalledExtension = value;
            OnPropertyChanged();
            if (value != null)
            {
                ExtensionIdText = value.ExtensionId;
            }

            RaiseOperationStateChanged();
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
            RaiseOperationStateChanged();
        }
    }

    public bool IsNotBusy => !IsBusy;

    public bool CanFetchExtensionMetadata => IsNotBusy && TryNormalizeExtensionIdText(out _);

    public bool CanInstallExtension => IsNotBusy && TryNormalizeExtensionIdText(out _);

    public bool CanUninstallSelectedExtension => IsNotBusy && ResolveUninstallTargetExtensionId() != null;

    public bool CanUseCurrentStoreExtensionId => IsNotBusy && !string.IsNullOrWhiteSpace(CurrentStoreExtensionId);

    public string CurrentStoreExtensionId
    {
        get => _currentStoreExtensionId;
        private set
        {
            if (string.Equals(_currentStoreExtensionId, value, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _currentStoreExtensionId = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CurrentStoreExtensionStatus));
            RaiseOperationStateChanged();
        }
    }

    public string CurrentStoreExtensionStatus => string.IsNullOrWhiteSpace(CurrentStoreExtensionId)
        ? "No Chrome Web Store extension detected on the current page."
        : $"Detected extension id: {CurrentStoreExtensionId}";

    public string StatusText
    {
        get => _statusText;
        private set
        {
            if (_statusText == value)
            {
                return;
            }

            _statusText = value;
            OnPropertyChanged();
        }
    }

    public override void Dispose()
    {
        StoreBrowserView.CurrentExtensionIdChanged -= OnStoreBrowserCurrentExtensionIdChanged;
        StoreBrowserView.Dispose();
        base.Dispose();
    }

    protected override void InternalOnLoaded()
    {
        base.InternalOnLoaded();
        RefreshInstalledExtensions();
    }

    private void OnStoreBrowserCurrentExtensionIdChanged(object? sender, EventArgs e)
    {
        CurrentStoreExtensionId = StoreBrowserView.CurrentExtensionId;
    }

    private async void OnRefreshButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        await RunBusyAsync("Refreshing Chrome extensions", () =>
        {
            RefreshInstalledExtensions();
            StatusText = "Local extension list refreshed.";
            return Task.CompletedTask;
        });
    }

    private async void OnOpenStoreButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        await RunBusyAsync("Opening Chrome Web Store", async () =>
        {
            await StoreBrowserView.NavigateToDefaultStorePageAsync();
        });
    }

    private void OnUseCurrentStoreExtensionButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CurrentStoreExtensionId))
        {
            return;
        }

        ExtensionIdText = CurrentStoreExtensionId;
        StatusText = $"Using detected extension id {CurrentStoreExtensionId}.";
    }

    private async void OnFetchButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        await FetchMetadataAsync();
    }

    private async void OnInstallButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        if (!TryNormalizeExtensionIdText(out var extensionId))
        {
            return;
        }

        await RunBusyAsync("Installing Chrome extension package", async () =>
        {
            var installed = await PackageManager.InstallAsync(extensionId);
            RefreshInstalledExtensions(installed.ExtensionId);
            StatusText = $"Installed local package {installed.DisplayName} {installed.Version}.";
            ToastManager.PromptMessage(ToastMessage.Info(StatusText, false));
        });
    }

    private async void OnUninstallButtonClick(object sender, System.Windows.RoutedEventArgs e)
    {
        var extensionId = ResolveUninstallTargetExtensionId();
        if (extensionId == null)
        {
            return;
        }

        await RunBusyAsync("Uninstalling Chrome extension package", async () =>
        {
            var removed = await PackageManager.UninstallAsync(extensionId);
            RefreshInstalledExtensions();
            StatusText = removed
                ? $"Removed local package {extensionId}."
                : $"Local package {extensionId} was not installed.";
            ToastManager.PromptMessage(ToastMessage.Info(StatusText, false));
        });
    }

    private async Task FetchMetadataAsync()
    {
        if (!TryNormalizeExtensionIdText(out var extensionId))
        {
            return;
        }

        await RunBusyAsync("Fetching Chrome extension metadata", async () =>
        {
            CurrentMetadata = await PackageMetadataProvider.GetPackageMetadataAsync(extensionId);
            StatusText = CurrentMetadata.HasPackage
                ? $"Fetched package metadata for {CurrentMetadata.ExtensionId} {CurrentMetadata.Version}."
                : $"No downloadable package returned for {CurrentMetadata.ExtensionId}.";
        });
    }

    private void RefreshInstalledExtensions(string? selectedExtensionId = null)
    {
        selectedExtensionId ??= SelectedInstalledExtension?.ExtensionId;

        InstalledExtensions.Clear();
        foreach (var installed in PackageManager.GetInstalledExtensions())
        {
            InstalledExtensions.Add(installed);
        }

        OnPropertyChanged(nameof(InstalledExtensionsCount));

        SelectedInstalledExtension = InstalledExtensions.FirstOrDefault(t =>
                                      string.Equals(t.ExtensionId, selectedExtensionId,
                                          StringComparison.OrdinalIgnoreCase))
                                  ?? InstalledExtensions.FirstOrDefault();
        RaiseOperationStateChanged();
    }

    private async Task RunBusyAsync(string title, Func<Task> action)
    {
        if (IsBusy)
        {
            return;
        }

        IsBusy = true;
        var handler = AppBusyWindow.Enqueue();
        handler.Title = title;
        handler.ShowProgress = true;
        handler.IsProgressIndeterminate = true;

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            StatusText = ex.Message;
            ToastManager.PromptException(ex);
        }
        finally
        {
            handler.Close();
            IsBusy = false;
        }
    }

    private void RaiseOperationStateChanged()
    {
        OnPropertyChanged(nameof(CanFetchExtensionMetadata));
        OnPropertyChanged(nameof(CanInstallExtension));
        OnPropertyChanged(nameof(CanUninstallSelectedExtension));
        OnPropertyChanged(nameof(CanUseCurrentStoreExtensionId));
    }

    private string? ResolveUninstallTargetExtensionId()
    {
        if (TryNormalizeExtensionIdText(out var extensionId)
            && PackageManager.IsInstalled(extensionId))
        {
            return extensionId;
        }

        return SelectedInstalledExtension?.ExtensionId;
    }

    private bool TryNormalizeExtensionIdText(out string extensionId)
    {
        try
        {
            extensionId = ChromeWebStoreExtensionId.Normalize(ExtensionIdText);
            return true;
        }
        catch
        {
            extensionId = "";
            return false;
        }
    }
}

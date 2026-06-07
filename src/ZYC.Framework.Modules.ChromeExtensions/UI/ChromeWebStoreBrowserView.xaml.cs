using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;
using ZYC.Framework.Modules.ChromeExtensions.Commands;
using ZYC.Framework.WebView2.Menu;

namespace ZYC.Framework.Modules.ChromeExtensions.UI;

[Register]
internal sealed partial class ChromeWebStoreBrowserView
{
    private string _currentExtensionId = "";
    private ChromeExtensionPackageMetadata? _currentMetadata;
    private int _currentMetadataRequestVersion;
    private InstallDetectedExtensionCommand? _installDetectedExtensionCommand;
    private bool _isBusy;
    private bool _isCurrentMetadataLoading;

    public ChromeWebStoreBrowserView(
        ILifetimeScope lifetimeScope,
        IAppBusyWindow appBusyWindow,
        IToastManager toastManager,
        IAppLogger<ChromeWebStoreBrowserView> logger,
        IChromeExtensionPackageMetadataProvider packageMetadataProvider,
        IChromeExtensionPackageManager packageManager,
        ChromeExtensionManagerConfig chromeExtensionManagerConfig,
        ChromeExtensionPackageStoreEvents packageStoreEvents) : base(lifetimeScope)
    {
        AppBusyWindow = appBusyWindow;
        ToastManager = toastManager;
        Logger = logger;
        PackageMetadataProvider = packageMetadataProvider;
        PackageManager = packageManager;
        ChromeExtensionManagerConfig = chromeExtensionManagerConfig;
        PackageStoreEvents = packageStoreEvents;

        HomePageUri = ChromeExtensionManagerConfig.StoreHomeUri;
    }

    private IAppBusyWindow AppBusyWindow { get; }

    private IToastManager ToastManager { get; }

    private IAppLogger<ChromeWebStoreBrowserView> Logger { get; }

    private IChromeExtensionPackageMetadataProvider PackageMetadataProvider { get; }

    private IChromeExtensionPackageManager PackageManager { get; }

    private ChromeExtensionManagerConfig ChromeExtensionManagerConfig { get; }

    private ChromeExtensionPackageStoreEvents PackageStoreEvents { get; }

    protected override ExtendedMenuItem[] WebViewHostBaseExtendedMenuItems =>
    [
        new()
        {
            Title = "Install",
            Icon = "DownloadOutline",
            Command = InstallDetectedExtensionCommand,
            Localization = false
        }
    ];

    private InstallDetectedExtensionCommand InstallDetectedExtensionCommand =>
        _installDetectedExtensionCommand ??= new InstallDetectedExtensionCommand(this);

    public string CurrentExtensionId
    {
        get => _currentExtensionId;
        private set
        {
            if (string.Equals(_currentExtensionId, value, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            _currentExtensionId = value;
            _ = LoadCurrentMetadataAsync(value);
        }
    }

    private ChromeExtensionPackageMetadata? CurrentMetadata
    {
        get => _currentMetadata;
        set
        {
            if (ReferenceEquals(_currentMetadata, value))
            {
                return;
            }

            _currentMetadata = value;
            RaiseOperationStateChanged();
        }
    }

    private bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value)
            {
                return;
            }

            _isBusy = value;
            RaiseOperationStateChanged();
        }
    }

    private bool IsCurrentMetadataLoading
    {
        get => _isCurrentMetadataLoading;
        set
        {
            if (_isCurrentMetadataLoading == value)
            {
                return;
            }

            _isCurrentMetadataLoading = value;
            RaiseOperationStateChanged();
        }
    }

    public bool CanInstallDetectedExtension => !IsBusy
                                               && !IsCurrentMetadataLoading
                                               && CurrentMetadata?.HasPackage == true;

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        await NavigateAsync(ChromeExtensionManagerConfig.StoreHomeUri);
    }

    protected override void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        base.OnNavigationStarting(sender, e);
        UpdateCurrentExtensionId(e.Uri);
    }

    protected override void OnSourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        base.OnSourceChanged(sender, e);
        UpdateCurrentExtensionId(CoreWebView2.Source);
    }

    private void UpdateCurrentExtensionId(string? source)
    {
        if (!string.IsNullOrWhiteSpace(source)
            && Uri.TryCreate(source, UriKind.Absolute, out var uri)
            && ChromeExtensionId.TryParseFromStoreUri(uri, out var extensionId))
        {
            CurrentExtensionId = extensionId;
            return;
        }

        CurrentExtensionId = "";
    }

    private async Task LoadCurrentMetadataAsync(string extensionId)
    {
        var requestVersion = Interlocked.Increment(ref _currentMetadataRequestVersion);
        CurrentMetadata = null;

        if (string.IsNullOrWhiteSpace(extensionId))
        {
            IsCurrentMetadataLoading = false;
            return;
        }

        IsCurrentMetadataLoading = true;
        try
        {
            var metadata = await PackageMetadataProvider.GetPackageMetadataAsync(extensionId);
            if (requestVersion != _currentMetadataRequestVersion)
            {
                return;
            }

            CurrentMetadata = metadata;
        }
        catch (Exception ex)
        {
            if (requestVersion != _currentMetadataRequestVersion)
            {
                return;
            }

            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
        finally
        {
            if (requestVersion == _currentMetadataRequestVersion)
            {
                IsCurrentMetadataLoading = false;
            }
        }
    }

    public async Task InstallDetectedExtensionAsync()
    {
        var metadata = CurrentMetadata;
        if (metadata?.HasPackage != true)
        {
            return;
        }

        await RunBusyAsync("Installing Chrome extension package", async () =>
        {
            var installed = await PackageManager.InstallAsync(metadata.ExtensionId);
            PackageStoreEvents.RaiseInstalledExtensionsChanged();
            ToastManager.PromptMessage(
                ToastMessage.Info($"Installed local package {installed.DisplayName} {installed.Version}.", false));
        });
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
        _installDetectedExtensionCommand?.RaiseCanExecuteChanged();
    }
}
using System.Collections.ObjectModel;
using System.Windows;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.BusyWindow;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;

namespace ZYC.Framework.Modules.ChromeExtensions.UI;

[Register]
internal partial class ChromeExtensionManagerView
{
    private bool _isBusy;
    private ChromeInstalledExtension? _selectedInstalledExtension;

    public ChromeExtensionManagerView(
        IDialogManager dialogManager,
        IAppBusyWindow appBusyWindow,
        IToastManager toastManager,
        IAppLogger<ChromeExtensionManagerView> logger,
        IChromeExtensionPackageManager packageManager,
        ChromeExtensionPackageStoreEvents packageStoreEvents)
    {
        DialogManager = dialogManager;
        AppBusyWindow = appBusyWindow;
        ToastManager = toastManager;
        Logger = logger;
        PackageManager = packageManager;
        PackageStoreEvents = packageStoreEvents;

        PackageStoreEvents.InstalledExtensionsChanged += OnInstalledExtensionsChanged;
    }

    private IDialogManager DialogManager { get; }

    private IAppBusyWindow AppBusyWindow { get; }

    private IToastManager ToastManager { get; }

    private IAppLogger<ChromeExtensionManagerView> Logger { get; }

    private IChromeExtensionPackageManager PackageManager { get; }

    private ChromeExtensionPackageStoreEvents PackageStoreEvents { get; }

    public ObservableCollection<ChromeInstalledExtension> InstalledExtensions { get; } = new();

    public int InstalledExtensionsCount => InstalledExtensions.Count;

    public ChromeInstalledExtension? SelectedInstalledExtension
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

    public bool CanUninstallSelectedExtension => IsNotBusy && SelectedInstalledExtension != null;

    public override void Dispose()
    {
        PackageStoreEvents.InstalledExtensionsChanged -= OnInstalledExtensionsChanged;
        base.Dispose();
    }

    protected override void InternalOnLoaded()
    {
        base.InternalOnLoaded();
        RefreshInstalledExtensions();
    }

    private void OnOpenStoreButtonClick(object sender, RoutedEventArgs e)
    {
        DialogManager.Show<ChromeWebStoreBrowserDialogWindow>();
    }

    private async void OnUninstallButtonClick(object sender, RoutedEventArgs e)
    {
        var extensionId = SelectedInstalledExtension?.ExtensionId;
        if (string.IsNullOrWhiteSpace(extensionId))
        {
            return;
        }

        await RunBusyAsync("Uninstalling Chrome extension package", async () =>
        {
            var removed = await PackageManager.UninstallAsync(extensionId);
            RefreshInstalledExtensions();
            PackageStoreEvents.RaiseInstalledExtensionsChanged();

            var message = removed
                ? $"Removed local package {extensionId}."
                : $"Local package {extensionId} was not installed.";
            ToastManager.PromptMessage(ToastMessage.Info(message, false));
        });
    }

    private void OnInstalledExtensionsChanged(object? sender, EventArgs e)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(() => RefreshInstalledExtensions());
            return;
        }

        RefreshInstalledExtensions();
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
        OnPropertyChanged(nameof(CanUninstallSelectedExtension));
    }
}
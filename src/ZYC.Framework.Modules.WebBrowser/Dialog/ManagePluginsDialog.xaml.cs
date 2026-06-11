using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;
using ZYC.Framework.Modules.WebBrowser.Abstractions;
using ZYC.Framework.WebView2;

namespace ZYC.Framework.Modules.WebBrowser.Dialog;

[Register]
internal partial class ManagePluginsDialog : INotifyPropertyChanged
{
    private string _searchText = "";

    public ManagePluginsDialog(
        ILogger<ManagePluginsDialog> logger,
        IAppContext appContext,
        IToastManager toastManager,
        WebBrowserConfig webBrowserConfig,
        IChromeExtensionPackageManager chromeExtensionPackageManager)
    {
        Logger = logger;
        AppContext = appContext;
        ToastManager = toastManager;
        WebBrowserConfig = webBrowserConfig;
        ChromeExtensionPackageManager = chromeExtensionPackageManager;
        DataContext = this;

        InitializeComponent();
        RefreshPluginItems();
    }

    public ObservableCollection<ManagePluginItem> PluginItems { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
            {
                return;
            }

            _searchText = value;
            OnPropertyChanged();
            RefreshPluginItems();
        }
    }

    public bool HasPluginItems => PluginItems.Count > 0;

    public string SummaryText
    {
        get
        {
            var loadedCount = PluginItems.Count(t => t.IsLoaded);
            return $"Plugins {loadedCount}/{PluginItems.Count}";
        }
    }

    private ILogger<ManagePluginsDialog> Logger { get; }

    private IAppContext AppContext { get; }

    private IToastManager ToastManager { get; }

    private WebBrowserConfig WebBrowserConfig { get; }

    private IChromeExtensionPackageManager ChromeExtensionPackageManager { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnRefreshButtonClick(object sender, RoutedEventArgs e)
    {
        RefreshPluginItems();
    }

    private void OnAddButtonClick(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not ManagePluginItem item)
        {
            return;
        }

        try
        {
            var paths = WebBrowserPluginArgumentTools
                .GetConfiguredExtensionPaths(WebBrowserConfig.CustomBrowserArguments)
                .ToList();
            if (!paths.Any(path => WebBrowserPluginArgumentTools.SamePath(path, item.UnpackedPath)))
            {
                paths.Add(item.UnpackedPath);
            }

            CommitConfiguredExtensionPaths(paths);
            RefreshPluginItems();
            ToastManager.PromptMessage(ToastMessage.Info($"Added browser plugin {item.DisplayName}.", false));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void OnRemoveButtonClick(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not ManagePluginItem item)
        {
            return;
        }

        try
        {
            var paths = WebBrowserPluginArgumentTools
                .GetConfiguredExtensionPaths(WebBrowserConfig.CustomBrowserArguments)
                .Where(path => !WebBrowserPluginArgumentTools.SamePath(path, item.UnpackedPath))
                .ToArray();

            CommitConfiguredExtensionPaths(paths);
            RefreshPluginItems();
            ToastManager.PromptMessage(ToastMessage.Info($"Removed browser plugin {item.DisplayName}.", false));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void RefreshPluginItems()
    {
        try
        {
            var configuredPaths = WebBrowserPluginArgumentTools
                .GetConfiguredExtensionPaths(WebBrowserConfig.CustomBrowserArguments)
                .ToArray();
            var searchText = SearchText.Trim();

            PluginItems.Clear();
            foreach (var installed in ChromeExtensionPackageManager.GetInstalledExtensions()
                         .Where(extension => IsMatch(extension, searchText)))
            {
                PluginItems.Add(new ManagePluginItem(
                    installed,
                    configuredPaths.Any(path => WebBrowserPluginArgumentTools.SamePath(path, installed.UnpackedPath))));
            }

            OnPropertyChanged(nameof(HasPluginItems));
            OnPropertyChanged(nameof(SummaryText));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private void CommitConfiguredExtensionPaths(IEnumerable<string> paths)
    {
        SetConfiguredExtensionPaths(WebBrowserConfig, paths);
        AppContext.SaveAllConfig();
    }


    public static void SetConfiguredExtensionPaths(
        WebBrowserConfig webBrowserConfig,
        IEnumerable<string> paths)
    {
        webBrowserConfig.CustomBrowserArguments = WebBrowserPluginArgumentTools.ReplaceConfiguredExtensionPaths(
            webBrowserConfig.CustomBrowserArguments,
            paths);
    }

    private static bool IsMatch(ChromeInstalledExtension extension, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return true;
        }

        return Contains(extension.DisplayName, searchText)
               || Contains(extension.ExtensionId, searchText)
               || Contains(extension.UnpackedPath, searchText);
    }

    private static bool Contains(string value, string searchText)
    {
        return value.Contains(searchText, StringComparison.OrdinalIgnoreCase);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
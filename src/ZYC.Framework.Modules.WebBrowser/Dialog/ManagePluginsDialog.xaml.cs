using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using Microsoft.Extensions.Logging;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser.Dialog;

[Register]
internal partial class ManagePluginsDialog : INotifyPropertyChanged
{
    private const string LoadExtensionArgumentName = "--load-extension";
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
            var paths = GetConfiguredExtensionPaths().ToList();
            if (!paths.Any(path => SamePath(path, item.UnpackedPath)))
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
            var paths = GetConfiguredExtensionPaths()
                .Where(path => !SamePath(path, item.UnpackedPath))
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
            var configuredPaths = GetConfiguredExtensionPaths().ToArray();
            var searchText = SearchText.Trim();

            PluginItems.Clear();
            foreach (var installed in ChromeExtensionPackageManager.GetInstalledExtensions()
                         .Where(extension => IsMatch(extension, searchText)))
            {
                PluginItems.Add(new ManagePluginItem(
                    installed,
                    configuredPaths.Any(path => SamePath(path, installed.UnpackedPath))));
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
        var configuredPaths = paths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .GroupBy(NormalizePath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToArray();

        var customBrowserArguments = WebBrowserConfig.CustomBrowserArguments
            .Where(argument => !IsLoadExtensionArgument(argument))
            .ToList();

        if (configuredPaths.Length > 0)
        {
            customBrowserArguments.Add(BuildLoadExtensionArgument(configuredPaths));
        }

        WebBrowserConfig.CustomBrowserArguments = customBrowserArguments.ToArray();
        AppContext.SaveAllConfig();
    }

    private IReadOnlyList<string> GetConfiguredExtensionPaths()
    {
        return WebBrowserConfig.CustomBrowserArguments
            .Where(IsLoadExtensionArgument)
            .SelectMany(ReadLoadExtensionArgumentPaths)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string BuildLoadExtensionArgument(IEnumerable<string> paths)
    {
        var value = string.Join(",", paths.Select(path => path.Trim().Trim('"')));
        return $"{LoadExtensionArgumentName}=\"{value}\"";
    }

    private static IEnumerable<string> ReadLoadExtensionArgumentPaths(string argument)
    {
        var trimmed = argument.Trim();
        var equalsIndex = trimmed.IndexOf('=');
        if (equalsIndex < 0 || equalsIndex >= trimmed.Length - 1)
        {
            return Array.Empty<string>();
        }

        var value = trimmed[(equalsIndex + 1)..].Trim().Trim('"', '\'');
        return value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(path => path.Trim().Trim('"', '\''));
    }

    private static bool IsLoadExtensionArgument(string argument)
    {
        var trimmed = argument.Trim();
        if (!trimmed.StartsWith(LoadExtensionArgumentName, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return trimmed.Length == LoadExtensionArgumentName.Length
               || trimmed[LoadExtensionArgumentName.Length] == '=';
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

    private static bool SamePath(string left, string right)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return false;
        }

        return string.Equals(NormalizePath(left), NormalizePath(right), StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizePath(string path)
    {
        var normalized = path.Trim().Trim('"', '\'');
        try
        {
            normalized = Path.GetFullPath(normalized);
        }
        catch
        {
            // Keep the raw value if it is not a normal file-system path.
        }

        return normalized.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
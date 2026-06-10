using System.IO;
using Autofac;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;
using ZYC.Framework.Modules.WebBrowser.Abstractions;
using ZYC.Framework.WebView2;
using ZYC.Framework.WebView2.Menu;

namespace ZYC.Framework.Modules.WebBrowser.UI;

internal partial class WebBrowserView
{
    private const string ChromeExtensionUriScheme = "chrome-extension";

    protected override PluginExtendedMenuItem[] GetPluginMenuItems()
    {
        var extendedMenuItems = new List<PluginExtendedMenuItem>();
        if (LifetimeScope.TryResolve<IChromeExtensionPackageManager>(out var chromeExtensionPackageManager))
        {
            extendedMenuItems.Add(LifetimeScope.Resolve<ManagePluginsExtendItem>());

            var navigatePluginPageCommand = CreateNavigatePluginPageCommand();
            foreach (var extension in GetEnabledInstalledExtensions(chromeExtensionPackageManager))
            {
                AddPluginPageMenuItem(
                    extendedMenuItems,
                    navigatePluginPageCommand,
                    extension,
                    "Options",
                    extension.PopupPagePath,
                    "PuzzleOutline");
                AddPluginPageMenuItem(
                    extendedMenuItems,
                    navigatePluginPageCommand,
                    extension,
                    "Options",
                    extension.OptionsPagePath,
                    "PuzzleOutline");
            }
        }

        return extendedMenuItems.ToArray();
    }

    private IEnumerable<ChromeInstalledExtension> GetEnabledInstalledExtensions(
        IChromeExtensionPackageManager chromeExtensionPackageManager)
    {
        var webBrowserConfig = LifetimeScope.Resolve<WebBrowserConfig>();

        var configuredPaths = WebBrowserPluginArgumentTools
            .GetConfiguredExtensionPaths(webBrowserConfig.CustomBrowserArguments);

        return chromeExtensionPackageManager.GetInstalledExtensions()
            .Where(extension => configuredPaths.Any(path =>
                WebBrowserPluginArgumentTools.SamePath(path, extension.UnpackedPath)));
    }

    private RelayCommand CreateNavigatePluginPageCommand()
    {
        return new RelayCommand(
            parameter => parameter is PluginPageNavigationTarget target
                         && !string.IsNullOrWhiteSpace(target.PagePath),
            parameter =>
            {
                if (parameter is PluginPageNavigationTarget target)
                {
                    _ = NavigatePluginPageAsync(target);
                }
            });
    }

    private async Task NavigatePluginPageAsync(PluginPageNavigationTarget target)
    {
        try
        {
            var pageUrl = CreatePluginPageUrl(target);
            if (string.IsNullOrWhiteSpace(pageUrl))
            {
                ToastManager.PromptMessage(
                    ToastMessage.Warn($"Unable to resolve plugin page for {target.Extension.DisplayName}.", false));
                return;
            }

            await TabManager.NavigateAsync(pageUrl);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }

    private string? CreatePluginPageUrl(PluginPageNavigationTarget target)
    {
        if (Uri.TryCreate(target.PagePath, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.Scheme.Equals(ChromeExtensionUriScheme, StringComparison.OrdinalIgnoreCase)
                ? absoluteUri.ToString()
                : null;
        }

        var runtimeExtensionId = TryGetLoadedExtensionId(target.Extension)
                                 ?? target.Extension.ExtensionId;
        if (string.IsNullOrWhiteSpace(runtimeExtensionId))
        {
            return null;
        }

        var normalizedPagePath = target.PagePath.Trim()
            .Replace(Path.DirectorySeparatorChar, '/')
            .Replace(Path.AltDirectorySeparatorChar, '/')
            .TrimStart('/');
        return string.IsNullOrWhiteSpace(normalizedPagePath)
            ? null
            : $"{ChromeExtensionUriScheme}://{runtimeExtensionId}/{normalizedPagePath}";
    }

    private string? TryGetLoadedExtensionId(ChromeInstalledExtension extension)
    {
        return CoreWebView2BrowserExtensions
                   .Where(loadedExtension => loadedExtension.IsEnabled)
                   .FirstOrDefault(loadedExtension =>
                       string.Equals(loadedExtension.Id, extension.ExtensionId, StringComparison.OrdinalIgnoreCase))
                   ?.Id
               ?? CoreWebView2BrowserExtensions
                   .Where(loadedExtension => loadedExtension.IsEnabled)
                   .FirstOrDefault(loadedExtension =>
                       string.Equals(loadedExtension.Name, extension.Name, StringComparison.OrdinalIgnoreCase))
                   ?.Id;
    }

    private static void AddPluginPageMenuItem(
        List<PluginExtendedMenuItem> extendedMenuItems,
        RelayCommand navigatePluginPageCommand,
        ChromeInstalledExtension extension,
        string pageTitle,
        string pagePath,
        string icon)
    {
        if (string.IsNullOrWhiteSpace(pagePath))
        {
            return;
        }

        extendedMenuItems.Add(
            new PluginExtendedMenuItem
            {
                Title = $"{extension.DisplayName} {pageTitle}",
                Icon = icon,
                Command = navigatePluginPageCommand,
                CommandParameter = new PluginPageNavigationTarget(extension, pagePath),
                Localization = false
            });
    }

    private sealed record PluginPageNavigationTarget(
        ChromeInstalledExtension Extension,
        string PagePath);
}
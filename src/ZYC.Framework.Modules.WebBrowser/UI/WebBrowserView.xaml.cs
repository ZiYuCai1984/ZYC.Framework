using System.IO;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Core.Commands;
using ZYC.Framework.Modules.ChromeExtensions.Abstractions;
using ZYC.Framework.Modules.WebBrowser.Abstractions;
using ZYC.Framework.WebView2;
using ZYC.Framework.WebView2.Menu;

namespace ZYC.Framework.Modules.WebBrowser.UI;

[Register]
internal partial class WebBrowserView
{
    private const string ChromeExtensionUriScheme = "chrome-extension";

    public WebBrowserView(
        IToastManager toastManager,
        ILogger<WebBrowserView> logger,
        IWebBrowserUriPolicy webBrowserUriPolicy,
        ITabManager tabManager,
        ILifetimeScope lifetimeScope,
        Uri uri,
        IWebTabItemInstance instance,
        WebBrowserConfig webBrowserConfig) : base(lifetimeScope)
    {
        ToastManager = toastManager;
        Logger = logger;
        WebBrowserUriPolicy = webBrowserUriPolicy;
        TabManager = tabManager;
        Uri = uri;
        Instance = instance;
        WebBrowserConfig = webBrowserConfig;

        CustomBrowserArguments.AddRange(webBrowserConfig.CustomBrowserArguments);
    }

    private IToastManager ToastManager { get; }

    private ILogger<WebBrowserView> Logger { get; }

    private IWebBrowserUriPolicy WebBrowserUriPolicy { get; }

    private ITabManager TabManager { get; }

    private Uri Uri { get; }

    private IWebTabItemInstance Instance { get; }

    private WebBrowserConfig WebBrowserConfig { get; }

    protected override bool IsApplyFaviconChanged => true;

    public override string HomePageUri => WebBrowserConfig.StartupUri;

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        await NavigateAsync(Uri);
    }

    protected override void OnDocumentTitleChanged(object? sender, object e)
    {
        base.OnDocumentTitleChanged(sender, e);
        Instance.SetTitle(CoreWebView2.DocumentTitle);
    }

    protected override Task InternalFaviconChangedAsync(object? sender, string base64)
    {
        Instance.SetIcon(base64);
        return Task.CompletedTask;
    }

    protected override ExtendedMenuItem[] GetPluginMenuItems()
    {
        var extendedMenuItems = new List<ExtendedMenuItem>();
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
                    "Popup",
                    extension.PopupPagePath,
                    "PuzzleOutline");
                AddPluginPageMenuItem(
                    extendedMenuItems,
                    navigatePluginPageCommand,
                    extension,
                    "Options",
                    extension.OptionsPagePath,
                    "CogOutline");
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
        List<ExtendedMenuItem> extendedMenuItems,
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
            new ExtendedMenuItem
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

    protected override async void OnSourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        //!WARNING Replaced the OnNavigationStarting
        try
        {
            base.OnSourceChanged(sender, e);

            var target = CoreWebView2.Source;
            await Instance.TabInternalNavigatingAsync(new Uri(target));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }


    protected override async void OnNewWindowRequested(
        object? sender,
        CoreWebView2NewWindowRequestedEventArgs e)
    {
        try
        {
            //!WARNING There appears to be a bug here.
            e.Handled = true;
            if (!WebBrowserUriPolicy.IsAllowed(new Uri(e.Uri)))
            {
                return;
            }

            await TabManager.NavigateAsync(e.Uri);
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
            ToastManager.PromptException(ex);
        }
    }
}

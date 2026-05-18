using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Abstractions.Notification.Toast;
using ZYC.Framework.Abstractions.Tab;
using ZYC.Framework.Modules.WebBrowser.Abstractions;

namespace ZYC.Framework.Modules.WebBrowser.UI;

[Register]
internal partial class WebBrowserView
{
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

    //protected override async void OnNavigationStarting(
    //    object? sender,
    //    CoreWebView2NavigationStartingEventArgs e)
    //{
    //    try
    //    {
    //        base.OnNavigationStarting(sender, e);

    //        var target = e.Uri;
    //        await Instance.TabInternalNavigatingAsync(new Uri(target));
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.Error(ex);
    //        ToastManager.PromptException(ex);
    //    }
    //}

    protected override async void OnSourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        //!WARNING Pending test
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
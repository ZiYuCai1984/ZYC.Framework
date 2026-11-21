using System.IO;
using System.Net.Http;
using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Abstractions;
using ZYC.Framework.WebView2;

namespace __PROJECT_NAME__.UI;

/// <summary>
///     Hosts the React app from ClientApp in a WebView2 tab.
/// </summary>
[Register]
internal class __PROJECT_SHORT_NAME__View : WebViewHostBase
{
    /// <summary>
    ///     Folder next to this assembly that receives ClientApp/dist at build time
    ///     (see ClientAppOutputFolder in the .csproj).
    /// </summary>
    private const string ClientAppFolderName = "ClientApp";

    /// <summary>
    ///     Virtual host mapped to <see cref="ClientAppFolderName" />. The reserved .example
    ///     top-level domain never reaches the network.
    /// </summary>
    private const string ClientAppHostName = "__PROJECT_WEB_NAME__.example";

    /// <summary>
    ///     Vite dev server (see ClientApp/vite.config.ts). Debug builds load it instead of the
    ///     built files while `npm run dev` is running, so React changes show up without a rebuild.
    /// </summary>
    private const string DevServerUri = "http://localhost:5173/";

    public __PROJECT_SHORT_NAME__View(
        ILifetimeScope lifetimeScope,
        IAppLogger<__PROJECT_SHORT_NAME__View> logger) : base(lifetimeScope)
    {
        Logger = logger;
        WebBridge = new WebBridge(lifetimeScope);
    }

    private IAppLogger<__PROJECT_SHORT_NAME__View> Logger { get; }

    /// <summary>
    ///     Answers the calls that ClientApp/src/webview.ts makes to [ExposeToWeb] services.
    /// </summary>
    private WebBridge WebBridge { get; }

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        var clientAppFolder = Path.Combine(
            Path.GetDirectoryName(typeof(__PROJECT_SHORT_NAME__View).Assembly.Location)!,
            ClientAppFolderName);

        CoreWebView2.SetVirtualHostNameToFolderMapping(
            ClientAppHostName,
            clientAppFolder,
            CoreWebView2HostResourceAccessKind.Allow);

        var startUri = $"https://{ClientAppHostName}/index.html";

#if DEBUG
        if (await IsDevServerRunningAsync())
        {
            startUri = DevServerUri;
        }
#endif

        HomePageUri = startUri;
        await NavigateAsync(startUri);
    }

    /// <summary>
    ///     Receives the calls that ClientApp/src/webview.ts sends (window.chrome.webview.postMessage)
    ///     and posts the replies back.
    /// </summary>
    protected override async void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        base.OnWebMessageReceived(sender, e);

        // Only trust messages from the page this view loaded, not from pages it navigated to later.
        if (!IsFromClientApp(e.Source))
        {
            return;
        }

        try
        {
            var reply = await WebBridge.HandleAsync(e.WebMessageAsJson);
            if (reply is not null && !Disposing && !IsCoreWebView2Null())
            {
                CoreWebView2.PostWebMessageAsJson(reply);
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
    }

    private bool IsFromClientApp(string source)
    {
        return Uri.TryCreate(source, UriKind.Absolute, out var sourceUri)
               && Uri.TryCreate(HomePageUri, UriKind.Absolute, out var homePageUri)
               && Uri.Compare(
                   sourceUri,
                   homePageUri,
                   UriComponents.SchemeAndServer,
                   UriFormat.Unescaped,
                   StringComparison.OrdinalIgnoreCase) == 0;
    }

#if DEBUG
    private async Task<bool> IsDevServerRunningAsync()
    {
        try
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(1));
            using var response = await InnerHttpClient.GetAsync(DevServerUri, cancellation.Token);
            return response.IsSuccessStatusCode;
        }
        catch (Exception exception) when (exception is HttpRequestException or TaskCanceledException)
        {
            return false;
        }
    }
#endif
}

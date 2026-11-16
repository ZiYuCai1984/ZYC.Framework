using System.IO;
using System.Net.Http;
using System.Text.Json;
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

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public __PROJECT_SHORT_NAME__View(
        ILifetimeScope lifetimeScope,
        IAppLogger<__PROJECT_SHORT_NAME__View> logger) : base(lifetimeScope)
    {
        Logger = logger;
    }

    private IAppLogger<__PROJECT_SHORT_NAME__View> Logger { get; }

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
    ///     Receives messages sent by ClientApp/src/webview.ts (window.chrome.webview.postMessage).
    /// </summary>
    protected override void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
    {
        base.OnWebMessageReceived(sender, e);

        // Only trust messages from the page this view loaded, not from pages it navigated to later.
        if (!IsFromClientApp(e.Source))
        {
            return;
        }

        try
        {
            var message = JsonSerializer.Deserialize<HostMessage>(e.WebMessageAsJson, JsonOptions);
            if (message?.Type == "ping")
            {
                PostToClientApp(new HostMessage(
                    "pong",
                    $"Hello from {ProductInfo.ProductName} {ProductInfo.Version} on .NET {Environment.Version}."));
            }
        }
        catch (JsonException exception)
        {
            Logger.Error(exception);
        }
    }

    private void PostToClientApp(HostMessage message)
    {
        CoreWebView2.PostWebMessageAsJson(JsonSerializer.Serialize(message, JsonOptions));
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

    /// <summary>
    ///     Message shape shared with ClientApp/src/webview.ts (serialized as camelCase JSON).
    /// </summary>
    private record HostMessage(string Type, string? Payload = null);
}

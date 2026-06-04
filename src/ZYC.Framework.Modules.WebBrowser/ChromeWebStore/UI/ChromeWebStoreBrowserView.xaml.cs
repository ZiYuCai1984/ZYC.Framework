using Autofac;
using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.WebView2;

namespace ZYC.Framework.Modules.WebBrowser.ChromeWebStore.UI;

[Register]
internal partial class ChromeWebStoreBrowserView : WebViewHostBase
{
    public const string DefaultStorePageUri =
        "https://chromewebstore.google.com/detail/block-site/nkedbnokglppcmiencngilkkhhnpcfjb";

    private string _currentExtensionId = "";

    public ChromeWebStoreBrowserView(ILifetimeScope lifetimeScope) : base(lifetimeScope)
    {
        HomePageUri = DefaultStorePageUri;
    }

    public event EventHandler? CurrentExtensionIdChanged;

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
            CurrentExtensionIdChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    protected override async Task InternalWebViewHostLoadedAsync()
    {
        await NavigateAsync(DefaultStorePageUri);
    }

    public async Task NavigateToDefaultStorePageAsync()
    {
        await NavigateAsync(DefaultStorePageUri);
    }

    protected override void OnSourceChanged(object? sender, CoreWebView2SourceChangedEventArgs e)
    {
        base.OnSourceChanged(sender, e);

        if (Uri.TryCreate(CoreWebView2.Source, UriKind.Absolute, out var uri)
            && ChromeWebStoreExtensionId.TryParseFromStoreUri(uri, out var extensionId))
        {
            CurrentExtensionId = extensionId;
            return;
        }

        CurrentExtensionId = "";
    }
}

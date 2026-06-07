using Microsoft.Web.WebView2.Core;
using ZYC.CoreToolkit.Extensions.Autofac.Attributes;
using ZYC.Framework.Core.Localizations;

namespace ZYC.Framework.WebView2.Dialog;

[Register]
internal partial class PluginsDialogView
{
    public PluginsDialogView(CoreWebView2BrowserExtension[]? coreWebView2BrowserExtensions)
    {
        CoreWebView2BrowserExtensions = coreWebView2BrowserExtensions
                                        ?? Array.Empty<CoreWebView2BrowserExtension>();
        InitializeComponent();
    }

    public CoreWebView2BrowserExtension[] CoreWebView2BrowserExtensions { get; }

    public int PluginInfoCount => CoreWebView2BrowserExtensions.Length;

    public string PluginSummary => $"{L.Translate("Plugins")} {PluginInfoCount}";
}
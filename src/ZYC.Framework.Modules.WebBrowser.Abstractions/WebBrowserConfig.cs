using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

/// <summary>
///     Configuration for the web browser module.
/// </summary>
public class WebBrowserConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the initial URI that will be opened when the browser starts.
    /// </summary>
    public string StartupUri { get; set; } = "https://google.com";

    /// <summary>
    ///     Gets or sets additional browser command-line arguments passed to the WebView2 environment.
    /// </summary>
    /// <remarks>
    ///     Use this for browser startup switches such as <c>--load-extension="C:\extensions\ext1,C:\extensions\ext2"</c>.
    /// </remarks>
    public string[] CustomBrowserArguments { get; set; } = [];
}
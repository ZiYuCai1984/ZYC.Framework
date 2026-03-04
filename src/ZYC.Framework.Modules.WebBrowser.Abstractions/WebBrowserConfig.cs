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
}
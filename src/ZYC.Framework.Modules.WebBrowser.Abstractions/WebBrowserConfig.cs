using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.WebBrowser.Abstractions;

public class WebBrowserConfig : IConfig
{
    public string StartupUri { get; set; } = "https://google.com";
}
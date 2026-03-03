using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.Translator.Abstractions;

/// <summary>
///     Represents the configuration settings for the translation service.
///     Implements <see cref="IConfig" /> for system-wide configuration management.
/// </summary>
public class TranslatorConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the base URL of the translation API provider.
    ///     Defaults to <c>http://127.0.0.1:5000</c>.
    /// </summary>
    public string Url { get; set; } = "http://127.0.0.1:5000";

    /// <summary>
    ///     Gets or sets a value indicating whether the translation service is enabled.
    /// </summary>
    public bool IsEnabled { get; set; }
}
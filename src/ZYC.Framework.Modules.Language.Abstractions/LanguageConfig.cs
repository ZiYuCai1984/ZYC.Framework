using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
/// Configuration options for language selection.
/// </summary>
[Hidden]
public class LanguageConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the current language.
    /// </summary>
    public LanguageType CurrentLanguage { get; set; } = LanguageType.en;
}
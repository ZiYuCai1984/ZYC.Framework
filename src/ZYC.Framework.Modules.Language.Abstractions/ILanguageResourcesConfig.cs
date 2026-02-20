using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
///     Defines configuration for language resources.
/// </summary>
[Hidden]
public interface ILanguageResourcesConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the language resources keyed by language type.
    /// </summary>
    public Dictionary<LanguageType, Dictionary<string, string>> Resources { get; set; }
}
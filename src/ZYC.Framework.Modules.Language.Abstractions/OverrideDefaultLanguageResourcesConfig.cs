using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
///     Stores user overrides for built-in language resources.
/// </summary>
public class OverrideDefaultLanguageResourcesConfig : ILanguageResourcesConfig
{
    /// <summary>
    ///     Gets or sets the language resources keyed by language type.
    /// </summary>
    public Dictionary<LanguageType, Dictionary<string, string>> Resources { get; set; } = new();
}

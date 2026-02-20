using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
///     Default implementation of language resource configuration.
/// </summary>
public class DefaultLanguageResourcesConfig : ILanguageResourcesConfig
{
    /// <summary>
    ///     Gets or sets the language resources keyed by language type.
    /// </summary>
    public Dictionary<LanguageType, Dictionary<string, string>> Resources { get; set; } = new();
}
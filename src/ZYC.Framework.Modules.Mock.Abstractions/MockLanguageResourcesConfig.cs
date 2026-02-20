using ZYC.Framework.Modules.Language.Abstractions;
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Mock.Abstractions;

/// <summary>
///     Provides mock language resources for testing translations.
/// </summary>
public class MockLanguageResourcesConfig : ILanguageResourcesConfig
{
    /// <summary>
    ///     Gets or sets the language resources keyed by language and resource key.
    /// </summary>
    public Dictionary<LanguageType, Dictionary<string, string>> Resources { get; set; } = new();
}
using ZYC.Framework.Modules.Translator.Abstractions;

namespace ZYC.Framework.Modules.Language.Abstractions;

/// <summary>
///     Represents a flattened localization resource entry for editing.
/// </summary>
public sealed class LanguageResourceEntry
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="LanguageResourceEntry" /> class.
    /// </summary>
    /// <param name="languageType">The language of the resource entry.</param>
    /// <param name="key">The source text key.</param>
    /// <param name="value">The localized value.</param>
    /// <param name="sourceName">The resource configuration that supplies the current value.</param>
    /// <param name="exists">Whether the selected language already has a value for the key.</param>
    /// <param name="canEdit">Whether the entry can be edited.</param>
    public LanguageResourceEntry(
        LanguageType languageType,
        string key,
        string value,
        string sourceName,
        bool exists,
        bool canEdit)
    {
        LanguageType = languageType;
        Key = key;
        Value = value;
        SourceName = sourceName;
        Exists = exists;
        CanEdit = canEdit;
    }

    /// <summary>
    ///     Gets the language of the resource entry.
    /// </summary>
    public LanguageType LanguageType { get; }

    /// <summary>
    ///     Gets the source text key.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the localized value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets the resource configuration that supplies the current value.
    /// </summary>
    public string SourceName { get; }

    /// <summary>
    ///     Gets a value indicating whether the selected language already has a value for the key.
    /// </summary>
    public bool Exists { get; }

    /// <summary>
    ///     Gets a value indicating whether the entry can be edited.
    /// </summary>
    public bool CanEdit { get; }
}

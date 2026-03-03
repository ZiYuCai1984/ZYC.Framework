using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Modules.Settings.Abstractions;

namespace ZYC.Framework.Modules.Mock.Abstractions;

/// <summary>
///     Defines mock configuration settings for sample scenarios.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class MockConfig : IConfig
{
    /// <summary>
    ///     Gets or sets long-form text for mock configuration testing.
    /// </summary>
    [MultilineText]
    public string LongText { get; set; } = "";

    /// <summary>
    ///     Gets or sets mock main menu visible.
    /// </summary>
    [MultilineText]
    public bool IsMainMenuVisible { get; set; } = true;
}
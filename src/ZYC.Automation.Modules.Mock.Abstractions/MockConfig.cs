using PropertyChanged;
using ZYC.Automation.Modules.Settings.Abstractions;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Automation.Modules.Mock.Abstractions;

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


    public bool IsMainMenuVisible { get; set; } = true;
}
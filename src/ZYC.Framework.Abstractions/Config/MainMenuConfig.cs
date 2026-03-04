using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration settings for the main menu component.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class MainMenuConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the main menu is visible in the user interface.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}
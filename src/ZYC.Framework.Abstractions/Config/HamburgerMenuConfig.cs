using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration settings for the hamburger menu component.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class HamburgerMenuConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the hamburger menu is visible in the user interface.
    /// </summary>
    public bool IsVisible { get; set; } = false;
}
using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for the application window title visibility.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class WindowTitleConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the window title is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}
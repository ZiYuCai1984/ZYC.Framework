using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for the application status bar display settings.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class StatusBarConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the status bar is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for the toast notification system.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
public class ToastConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the maximum number of toast notifications that can be displayed simultaneously.
    /// </summary>
    public int MaxToasts { get; set; } = 7;

    /// <summary>
    ///     Gets or sets the corner where toast notifications are displayed.
    /// </summary>
    public ToastPlacement Placement { get; set; } = ToastPlacement.BottomRight;
}

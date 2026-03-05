using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for the navigation history and behavior.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
public class NavigationConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the maximum number of navigation history entries to retain.
    /// </summary>
    public int MaxHistoryNum { get; set; } = 10;
}
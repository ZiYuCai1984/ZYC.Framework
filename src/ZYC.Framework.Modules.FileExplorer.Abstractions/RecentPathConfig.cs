using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

/// <summary>
///     Represents the configuration for the recent path feature.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class RecentPathConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the maximum number of recent paths to keep.
    /// </summary>
    public int MaxCount { get; set; } = 10;
}
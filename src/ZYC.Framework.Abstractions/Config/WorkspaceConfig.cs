using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for the workspace settings.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class WorkspaceConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the workspace menu is visible.
    /// </summary>
    public bool IsMenuVisible { get; set; } = true;

    /// <summary>
    ///     Gets or sets a value indicating whether the workspace empty index is visible.
    /// </summary>
    public bool IsWorkspaceEmptyIndexVisible { get; set; } = true;
}
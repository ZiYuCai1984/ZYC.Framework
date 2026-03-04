using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Configuration for the workspace-specific menu settings.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class WorkspaceMenuConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether the workspace menu is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}
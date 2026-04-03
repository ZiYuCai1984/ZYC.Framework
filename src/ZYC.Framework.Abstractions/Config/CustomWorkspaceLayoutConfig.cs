using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;
using ZYC.Framework.Abstractions.Workspace;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Stores all persisted user-defined workspace layout presets.
/// </summary>
[Hidden]
[AddINotifyPropertyChangedInterface]
public class CustomWorkspaceLayoutConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the collection of custom workspace layouts.
    /// </summary>
    public CustomWorkspaceLayout[] Layouts { get; set; } = [];
}

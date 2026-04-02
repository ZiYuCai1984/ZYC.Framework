using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

/// <summary>
///     Configuration settings for the file explorer functionality.
///     Implements the <see cref="IConfig" /> interface.
/// </summary>
public class FileExplorerConfig : IConfig
{
    /// <summary>
    ///     Gets or sets a value indicating whether to use the system's built-in file explorer.
    /// </summary>
    /// <value>
    ///     <c>true</c> to use the internal explorer; <c>false</c> to use a custom or external provider.
    ///     Default is <c>true</c>.
    /// </value>
    public bool UseBuiltInFileExplorer { get; set; } = true;
}
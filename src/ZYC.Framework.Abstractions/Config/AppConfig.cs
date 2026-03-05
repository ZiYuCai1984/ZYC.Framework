using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

/// <summary>
///     Represents the global configuration for the application.
/// </summary>
[AddINotifyPropertyChangedInterface]
public class AppConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the title of the application window.
    /// </summary>
    public string Title { get; set; } = ProductInfo.ProductName;

    /// <summary>
    ///     Gets or sets the minimum width of the main window.
    /// </summary>
    public int MinWidth { get; set; } = 800;

    /// <summary>
    ///     Gets or sets the minimum height of the main window.
    /// </summary>
    public int MinHeight { get; set; } = 600;

    /// <summary>
    ///     Gets or sets a value indicating whether global exceptions should be handled by the framework.
    /// </summary>
    public bool HandleGlobalException { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the application runs in debug mode.
    /// </summary>
    public bool DebugMode { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether a desktop shortcut should be created.
    /// </summary>
    public bool DesktopShortcut { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the application should start automatically at system startup.
    /// </summary>
    public bool StartAtBoot { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the application window should appear in the taskbar.
    /// </summary>
    public bool ShowInTaskbar { get; set; } = true;

    /// <summary>
    ///     Gets or sets the window corner preference used by the application.
    /// </summary>
    public CornerPreference CornerPreference { get; set; } = CornerPreference.DoNotRound;
}
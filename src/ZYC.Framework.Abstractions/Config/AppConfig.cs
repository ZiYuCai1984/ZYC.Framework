using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

[AddINotifyPropertyChangedInterface]
public class AppConfig : IConfig
{
    public string Title { get; set; } = ProductInfo.ProductName;

    public int MinWidth { get; set; } = 800;

    public int MinHeight { get; set; } = 600;

    public bool HandleGlobalException { get; set; }

    public bool DebugMode { get; set; }

    public bool DesktopShortcut { get; set; }

    public bool StartAtBoot { get; set; }

    public bool ShowInTaskbar { get; set; } = true;

    public CornerPreference CornerPreference { get; set; } = CornerPreference.DoNotRound;
}
using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Automation.Abstractions.Config;

[AddINotifyPropertyChangedInterface]
public class WindowTitleConfig : IConfig
{
    public bool IsVisible { get; set; } = true;
}
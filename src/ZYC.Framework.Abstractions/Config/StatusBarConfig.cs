using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

[AddINotifyPropertyChangedInterface]
public class StatusBarConfig : IConfig
{
    public bool IsVisible { get; set; } = true;
}
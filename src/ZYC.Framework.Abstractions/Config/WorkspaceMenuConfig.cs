using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

[AddINotifyPropertyChangedInterface]
public class WorkspaceMenuConfig : IConfig
{
    public bool IsVisible { get; set; } = true;
}
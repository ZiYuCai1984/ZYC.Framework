using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

[AddINotifyPropertyChangedInterface]
public class RecentPathConfig : IConfig
{
    public int MaxCount { get; set; } = 10;
}
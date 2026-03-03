using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

[AddINotifyPropertyChangedInterface]
public class RecentPathState : IState
{
    public string[] Paths { get; set; } = [];
}
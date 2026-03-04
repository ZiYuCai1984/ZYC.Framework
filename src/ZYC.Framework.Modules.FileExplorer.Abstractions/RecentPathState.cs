using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.FileExplorer.Abstractions;

#pragma warning disable CS1591

[AddINotifyPropertyChangedInterface]
public class RecentPathState : IState
{
    public string[] Paths { get; set; } = [];
}
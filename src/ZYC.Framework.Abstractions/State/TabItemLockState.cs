using PropertyChanged;
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.State;

#pragma warning disable CS1591

[AddINotifyPropertyChangedInterface]
public class TabItemLockState : IState
{
    public TabReference[] TabItems { get; set; } = [];
}
using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Tab;

namespace ZYC.Framework.Abstractions.State;

public class TabItemLockState : IState
{
    public TabReference[] TabItems { get; set; } = [];
}
using ZYC.Automation.Abstractions.Config.Attributes;
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Automation.Abstractions.State;

public class AppState : IState
{
    [SkipReset] public StartupTarget StartupTarget { get; set; } = StartupTarget.Main;
}
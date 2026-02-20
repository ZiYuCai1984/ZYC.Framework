using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Abstractions.State;

public class AppState : IState
{
    [SkipReset] public StartupTarget StartupTarget { get; set; } = StartupTarget.Main;
}
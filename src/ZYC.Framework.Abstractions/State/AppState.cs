using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Abstractions.State;

#pragma warning disable CS1591

public class AppState : IState
{
    [SkipReset] public string StartupVersion { get; set; } = ProductInfo.Version;
}
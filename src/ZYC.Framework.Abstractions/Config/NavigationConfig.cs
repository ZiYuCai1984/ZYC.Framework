using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

public class NavigationConfig : IConfig
{
    public int MaxHistoryNum { get; set; } = 10;
}
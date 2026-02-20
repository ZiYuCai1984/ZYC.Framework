using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Abstractions.Config;

[Hidden]
public class StarQuickBarConfig : IConfig
{
    /// <summary>
    ///     Format Uri|Icon
    /// </summary>
    public string[] Target { get; set; } = [];
}
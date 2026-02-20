using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Abstractions.Config;

public class ModuleConfig : IConfig
{
    public string[] DisabledAssemblyNames { get; set; } = [];

    public string[] AdditionalAssemblyNames { get; set; } = [];
}
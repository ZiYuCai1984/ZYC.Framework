using ZYC.CoreToolkit.Abstractions.Settings;
using ZYC.Framework.Abstractions.Config.Attributes;

namespace ZYC.Framework.Modules.Secrets.Abstractions;

/// <summary>
///     Marks a configuration object as a secrets configuration.
/// </summary>
[Hidden]
public interface ISecrets : IConfig
{
}
using ZYC.CoreToolkit.Abstractions.Settings;

namespace ZYC.Framework.Modules.CLI.Abstractions;

/// <summary>
///     Configuration options for the CLI module.
/// </summary>
public class CLIConfig : IConfig
{
    /// <summary>
    ///     Gets or sets the default command line executed at startup.
    /// </summary>
    public string StartupCommandLine { get; set; } = "cmd.exe";
}
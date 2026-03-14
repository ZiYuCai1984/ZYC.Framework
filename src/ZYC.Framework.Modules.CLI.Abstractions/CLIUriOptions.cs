using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.CLI.Abstractions;

public sealed record CLIUriOptions(
    [UriQueryName("startup")] string? StartupCommandLineOverride,
    [UriQueryName("exec")] IReadOnlyList<string> ExecCommands,
    [UriQueryName("type")] bool TypeOnly = false,
    [UriQueryName("text")] string? TypeText = null,
    [UriQueryName("focus")] bool FocusOnLoaded = true)
{
    public bool ShouldExecute => !TypeOnly;
}
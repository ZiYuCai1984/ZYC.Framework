using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.CLI.Abstractions;

/// <summary>
/// Defines CLI behavior parsed from URI query parameters.
/// </summary>
/// <param name="StartupCommandLineOverride">Overrides the startup command line.</param>
/// <param name="ExecCommands">Specifies commands to execute after the terminal is ready.</param>
/// <param name="TypeOnly">Indicates whether only text input should be sent without executing.</param>
/// <param name="TypeText">Specifies the text to send to the terminal.</param>
/// <param name="FocusOnLoaded">Indicates whether the view should receive focus after loading.</param>
public sealed record CLIUriOptions(
    [UriQueryName("startup")] string? StartupCommandLineOverride,
    [UriQueryName("exec")] IReadOnlyList<string> ExecCommands,
    [UriQueryName("type")] bool TypeOnly = false,
    [UriQueryName("text")] string? TypeText = null,
    [UriQueryName("focus")] bool FocusOnLoaded = true)
{
    /// <summary>
    /// Gets a value indicating whether the startup command should be executed.
    /// </summary>
    public bool ShouldExecute => !TypeOnly;
}

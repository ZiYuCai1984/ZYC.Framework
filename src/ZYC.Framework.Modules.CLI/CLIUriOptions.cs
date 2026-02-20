using ZYC.Framework.Abstractions;
using ZYC.Framework.Core;

namespace ZYC.Framework.Modules.CLI;

internal sealed record CLIUriOptions(
    [UriQueryName("startup")] string? StartupCommandLineOverride,
    [UriQueryName("exec")] IReadOnlyList<string> ExecCommands,
    [UriQueryName("type")] bool TypeOnly = false,
    [UriQueryName("text")] string? TypeText = null,
    [UriQueryName("focus")] bool FocusOnLoaded = true)
{
    public bool ShouldExecute => !TypeOnly;

    public static CLIUriOptions Parse(Uri uri)
    {
        return UriBinder.Bind<CLIUriOptions>(uri);
    }
}
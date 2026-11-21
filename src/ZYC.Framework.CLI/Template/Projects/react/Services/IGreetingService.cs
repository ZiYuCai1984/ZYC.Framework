using ZYC.Framework.Abstractions.MCP;
using ZYC.Framework.Abstractions.Web;

namespace __PROJECT_NAME__.Services;

/// <summary>
///     One contract for three callers: C# code resolves it from DI, the MCP server exposes it as tools,
///     and the React page calls it through the web bridge. The build generates
///     ClientApp/src/host-services.d.ts from it.
/// </summary>
[ExposeToWeb]
[ExposeToMCP]
public interface IGreetingService
{
    /// <summary>
    ///     Returns a greeting for the given name from the C# host.
    /// </summary>
    Task<HostGreeting> GreetAsync(string name);
}

/// <summary>
///     A greeting returned by <see cref="IGreetingService.GreetAsync" />.
/// </summary>
public record HostGreeting(string Message, string HostVersion, DateTimeOffset Time);

using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.MCP.Server.Abstractions;

public static class MCPServerModuleConstants
{
    public const string Host = "mcp";

    public const string Title = "MCPServer";

    public const string Icon = Base64IconResources.MCP;

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
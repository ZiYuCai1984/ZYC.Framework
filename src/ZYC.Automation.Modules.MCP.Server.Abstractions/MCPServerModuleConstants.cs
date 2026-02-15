using ZYC.Automation.Abstractions;

namespace ZYC.Automation.Modules.MCP.Server.Abstractions;

public static class MCPServerModuleConstants
{
    public const string Host = "mcpserver";

    public const string Title = "MCPServer";

    public const string Icon = Base64IconResources.MCP;

    public static Uri Uri => UriTools.CreateAppUri(Host);
}
using System.Text.Json;
using ModelContextProtocol.Protocol;

namespace ZYC.Framework.Modules.MCP.Server.UI;

internal class MCPToolItem
{
    private const string DefaultGroup = "Other";

    public MCPToolItem(Tool tool)
    {
        Name = tool.Name;
        Description = tool.Description ?? string.Empty;
        InputSchema = tool.InputSchema;
        InputSchemaText = MCPInspectorJson.Format(tool.InputSchema);

        var separatorIndex = Name.IndexOf('_');
        Group = separatorIndex > 0 ? Name[..separatorIndex] : DefaultGroup;
        ShortName = separatorIndex > 0 ? Name[(separatorIndex + 1)..] : Name;
    }

    public string Name { get; }

    public string ShortName { get; }

    public string Group { get; }

    public string Description { get; }

    public JsonElement InputSchema { get; }

    public string InputSchemaText { get; }
}
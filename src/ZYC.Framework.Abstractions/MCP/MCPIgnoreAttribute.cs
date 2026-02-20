namespace ZYC.Framework.Abstractions.MCP;

/// <summary>
///     Specifies that a method should be ignored by the MCP (Model Context Protocol) or related processing.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class MCPIgnoreAttribute : Attribute
{
    // This attribute does not require additional logic as it serves as a metadata marker.
}
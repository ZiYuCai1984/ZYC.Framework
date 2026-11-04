using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using ModelContextProtocol;
using ModelContextProtocol.Protocol;

namespace ZYC.Framework.Modules.MCP.Server.UI;

internal static class MCPInspectorJson
{
    private static readonly JsonSerializerOptions IndentedOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public static string Format(JsonElement element)
    {
        return element.ValueKind == JsonValueKind.Undefined
            ? string.Empty
            : JsonSerializer.Serialize(element, IndentedOptions);
    }

    public static string Serialize<T>(T value)
    {
        return Format(JsonSerializer.SerializeToElement(value, McpJsonUtilities.DefaultOptions));
    }

    public static Dictionary<string, JsonElement>? ParseArguments(string text)
    {
        return string.IsNullOrWhiteSpace(text)
            ? null
            : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(text);
    }

    public static string CreateArgumentsTemplate(JsonElement schema)
    {
        var arguments = new JsonObject();

        if (schema.ValueKind == JsonValueKind.Object &&
            schema.TryGetProperty("properties", out var properties) &&
            properties.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in properties.EnumerateObject())
            {
                arguments[property.Name] = CreateDefaultValue(property.Value);
            }
        }

        return arguments.ToJsonString(IndentedOptions);
    }

    public static string FormatContent(CallToolResult result)
    {
        var parts = new List<string>();

        foreach (var block in result.Content)
        {
            parts.Add(block is TextContentBlock textBlock
                ? FormatText(textBlock.Text)
                : Serialize(block));
        }

        if (parts.Count == 0 && result.StructuredContent is { } structuredContent)
        {
            parts.Add(Serialize(structuredContent));
        }

        return string.Join(Environment.NewLine + Environment.NewLine, parts);
    }

    private static string FormatText(string text)
    {
        var trimmed = text.TrimStart();
        if (!trimmed.StartsWith('{') && !trimmed.StartsWith('['))
        {
            return text;
        }

        try
        {
            using var document = JsonDocument.Parse(text);
            return Format(document.RootElement);
        }
        catch (JsonException)
        {
            return text;
        }
    }

    private static JsonNode? CreateDefaultValue(JsonElement schema)
    {
        if (schema.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        if (schema.TryGetProperty("default", out var defaultValue))
        {
            return JsonNode.Parse(defaultValue.GetRawText());
        }

        if (schema.TryGetProperty("enum", out var enumValues) &&
            enumValues.ValueKind == JsonValueKind.Array &&
            enumValues.GetArrayLength() > 0)
        {
            return JsonNode.Parse(enumValues[0].GetRawText());
        }

        return GetSchemaType(schema) switch
        {
            "string" => (JsonNode?)JsonValue.Create(string.Empty),
            "integer" or "number" => JsonValue.Create(0),
            "boolean" => JsonValue.Create(false),
            "array" => new JsonArray(),
            "object" => new JsonObject(),
            _ => null
        };
    }

    private static string? GetSchemaType(JsonElement schema)
    {
        if (!schema.TryGetProperty("type", out var type))
        {
            return null;
        }

        return type.ValueKind switch
        {
            JsonValueKind.String => type.GetString(),
            JsonValueKind.Array => type.EnumerateArray()
                .Where(t => t.ValueKind == JsonValueKind.String)
                .Select(t => t.GetString())
                .FirstOrDefault(t => t != "null"),
            _ => null
        };
    }
}
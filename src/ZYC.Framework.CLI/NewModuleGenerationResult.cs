namespace ZYC.Framework.CLI;

public class NewModuleGenerationResult
{
    public required string Target { get; init; }

    public required string SourceRoot { get; init; }

    public string? SlnxPath { get; init; }

    public required IReadOnlyList<string> GeneratedFiles { get; init; }
}
namespace ZYC.Framework.CLI;

public class NewProjectGenerationResult
{
    public required string Name { get; init; }

    public required string Template { get; init; }

    public required string OutputRoot { get; init; }

    public required IReadOnlyList<string> GeneratedFiles { get; init; }
}
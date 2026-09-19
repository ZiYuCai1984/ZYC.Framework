namespace ZYC.Framework.CLI;

public class NewModuleGenerationOptions
{
    public string Target { get; init; } = string.Empty;

    public string? SourceRoot { get; init; }

    public string? SlnxPath { get; init; }

    public bool Overwrite { get; init; }
}
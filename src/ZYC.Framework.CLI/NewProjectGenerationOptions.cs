namespace ZYC.Framework.CLI;

public class NewProjectGenerationOptions
{
    public string Name { get; init; } = string.Empty;

    public string Template { get; init; } = NewProjectGenerator.DefaultTemplateName;

    public string? OutputRoot { get; init; }

    public string? PackageVersion { get; init; }

    public bool Overwrite { get; init; }
}
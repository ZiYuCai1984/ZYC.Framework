using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ProductInfo = ZYC.Framework.Abstractions.ProductInfo;

namespace ZYC.Framework.CLI;

public sealed class NewProjectGenerationOptions
{
    public string Name { get; init; } = string.Empty;

    public string Template { get; init; } = NewProjectGenerator.DefaultTemplateName;

    public string? OutputRoot { get; init; }

    public string? PackageVersion { get; init; }

    public bool Overwrite { get; init; }
}

public sealed class NewProjectGenerationResult
{
    public required string Name { get; init; }

    public required string Template { get; init; }

    public required string OutputRoot { get; init; }

    public required IReadOnlyList<string> GeneratedFiles { get; init; }
}

public static class NewProjectGenerator
{
    public const string DefaultTemplateName = "minimal";

    private static readonly UTF8Encoding Utf8BomEncoding = new(true);

    private static readonly string[] SupportedTemplateNames =
    [
        DefaultTemplateName,
        "modular"
    ];

    private static readonly Regex ProjectNameRegex = new(
        @"^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)*$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly HashSet<string> TextTemplateFileNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ".editorconfig",
        ".gitignore"
    };

    private static readonly HashSet<string> TextTemplateFileExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs",
        ".csproj",
        ".json",
        ".md",
        ".props",
        ".slnx",
        ".targets",
        ".xaml",
        ".xml"
    };

    public static IReadOnlyList<string> TemplateNames => SupportedTemplateNames;

    public static NewProjectGenerationResult Generate(NewProjectGenerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var nameInfo = ParseProjectName(options.Name);
        var template = ResolveTemplateName(options.Template);
        var outputRoot = ResolveOutputRoot(options.OutputRoot, nameInfo.FullName);
        var packageVersion = ResolvePackageVersion(options.PackageVersion);
        var templateRoot = ResolveTemplateRoot(template);

        Directory.CreateDirectory(outputRoot);

        var replacements = CreateReplacements(nameInfo, packageVersion);
        var generatedFiles = Directory.GetFiles(templateRoot, "*", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .Select(file => GenerateFile(
                templateRoot,
                outputRoot,
                file,
                replacements,
                options.Overwrite))
            .ToList();

        return new NewProjectGenerationResult
        {
            Name = nameInfo.FullName,
            Template = template,
            OutputRoot = outputRoot,
            GeneratedFiles = generatedFiles
        };
    }

    private static ProjectNameInfo ParseProjectName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Project name is required.", nameof(name));
        }

        name = name.Trim();
        if (!ProjectNameRegex.IsMatch(name))
        {
            throw new ArgumentException(
                $"Project name '{name}' must be a valid dotted C# identifier, for example 'Acme.BookStore'.",
                nameof(name));
        }

        var shortName = name.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                            .Last();

        return new ProjectNameInfo(
            name,
            shortName,
            shortName.ToLowerInvariant());
    }

    private static string ResolveTemplateName(string? template)
    {
        template = string.IsNullOrWhiteSpace(template)
            ? DefaultTemplateName
            : template.Trim();

        var supportedTemplate = SupportedTemplateNames.FirstOrDefault(
            candidate => string.Equals(candidate, template, StringComparison.OrdinalIgnoreCase));

        if (supportedTemplate == null)
        {
            throw new ArgumentException(
                $"Unknown template '{template}'. Supported templates: {string.Join(", ", SupportedTemplateNames)}.",
                nameof(template));
        }

        return supportedTemplate;
    }

    private static string ResolveOutputRoot(string? outputRoot, string projectName)
    {
        if (string.IsNullOrWhiteSpace(outputRoot))
        {
            return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), projectName));
        }

        return Path.GetFullPath(outputRoot);
    }

    private static string ResolvePackageVersion(string? packageVersion)
    {
        return string.IsNullOrWhiteSpace(packageVersion)
            ? ProductInfo.Version
            : packageVersion.Trim();
    }

    private static string ResolveTemplateRoot(string template)
    {
        var templateRoot = Path.Combine(AppContext.BaseDirectory, "Template", "Projects", template);
        if (!Directory.Exists(templateRoot))
        {
            throw new DirectoryNotFoundException($"Project template root not found: '{templateRoot}'.");
        }

        return templateRoot;
    }

    private static IReadOnlyDictionary<string, string> CreateReplacements(
        ProjectNameInfo nameInfo,
        string packageVersion)
    {
        return new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["__PROJECT_NAME__"] = nameInfo.FullName,
            ["__PROJECT_SHORT_NAME__"] = nameInfo.ShortName,
            ["__PROJECT_HOST__"] = nameInfo.Host,
            ["__PACKAGE_VERSION__"] = packageVersion
        };
    }

    private static string GenerateFile(
        string templateRoot,
        string outputRoot,
        string templateFilePath,
        IReadOnlyDictionary<string, string> replacements,
        bool overwrite)
    {
        var templateRelativePath = Path.GetRelativePath(templateRoot, templateFilePath);
        var targetRelativePath = ReplaceTokens(templateRelativePath, replacements);
        var targetFilePath = Path.Combine(outputRoot, targetRelativePath);
        var targetFolder = Path.GetDirectoryName(targetFilePath)
                           ?? throw new InvalidOperationException($"Cannot resolve target folder: '{targetFilePath}'.");

        Directory.CreateDirectory(targetFolder);

        if (!overwrite && File.Exists(targetFilePath))
        {
            throw new IOException(
                $"Target file already exists: '{targetFilePath}'. Pass --overwrite to replace existing files.");
        }

        if (IsTextTemplateFile(templateFilePath))
        {
            var content = File.ReadAllText(templateFilePath);
            content = ReplaceTokens(content, replacements);
            content = NormalizeLineEndings(content);
            File.WriteAllText(targetFilePath, content, Utf8BomEncoding);
        }
        else
        {
            File.Copy(templateFilePath, targetFilePath, overwrite);
        }

        return targetFilePath;
    }

    private static string ReplaceTokens(string value, IReadOnlyDictionary<string, string> replacements)
    {
        foreach (var (token, replacement) in replacements)
        {
            value = value.Replace(token, replacement, StringComparison.Ordinal);
        }

        return value;
    }

    private static bool IsTextTemplateFile(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        if (TextTemplateFileNames.Contains(fileName))
        {
            return true;
        }

        return TextTemplateFileExtensions.Contains(Path.GetExtension(filePath));
    }

    private static string NormalizeLineEndings(string content)
    {
        return content.Replace("\r\n", "\n", StringComparison.Ordinal)
                      .Replace('\r', '\n')
                      .Replace("\n", "\r\n", StringComparison.Ordinal);
    }

    private sealed record ProjectNameInfo(
        string FullName,
        string ShortName,
        string Host);
}

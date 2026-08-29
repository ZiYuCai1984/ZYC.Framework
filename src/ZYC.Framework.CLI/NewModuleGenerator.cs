using System.Text;
using System.Xml.Linq;

namespace ZYC.Framework.CLI;

public sealed class NewModuleGenerationOptions
{
    public string Target { get; init; } = string.Empty;

    public string? SourceRoot { get; init; }

    public string? SlnxPath { get; init; }

    public bool Overwrite { get; init; }
}

public sealed class NewModuleGenerationResult
{
    public required string Target { get; init; }

    public required string SourceRoot { get; init; }

    public string? SlnxPath { get; init; }

    public required IReadOnlyList<string> GeneratedFiles { get; init; }
}

public static class NewModuleGenerator
{
    public const string TemplateName = "Chronosynchronicity";
    private static readonly UTF8Encoding Utf8BomEncoding = new(true);

    public static NewModuleGenerationResult Generate(NewModuleGenerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var target = NormalizeTarget(options.Target);
        var sourceRoot = ResolveSourceRoot(options.SourceRoot);
        var templateRoot = ResolveTemplateRoot();
        var moduleTemplateRoots = ResolveModuleTemplateRoots(templateRoot);
        var shortName = GetShortName(target);
        var slnxPath = ResolveSlnxPath(sourceRoot, options.SlnxPath);

        EnsureTargetDoesNotExist(sourceRoot, target, options.Overwrite);

        var generatedFiles = moduleTemplateRoots
            .SelectMany(root => Directory.GetFiles(root, "*.*", SearchOption.AllDirectories))
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .Select(file => GenerateFile(templateRoot, sourceRoot, file, target, shortName, options.Overwrite))
            .ToList();

        if (!string.IsNullOrWhiteSpace(slnxPath))
        {
            UpdateSlnx(sourceRoot, target, slnxPath);
        }

        return new NewModuleGenerationResult
        {
            Target = target,
            SourceRoot = sourceRoot,
            SlnxPath = slnxPath,
            GeneratedFiles = generatedFiles
        };
    }

    private static string NormalizeTarget(string target)
    {
        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentException("Target is required.", nameof(target));
        }

        target = target.Trim();

        const string modulePrefix = "ZYC.Framework.Modules.";
        if (target.StartsWith(modulePrefix, StringComparison.Ordinal))
        {
            target = target[modulePrefix.Length..];
        }

        const string abstractionsSuffix = ".Abstractions";
        if (target.EndsWith(abstractionsSuffix, StringComparison.Ordinal))
        {
            target = target[..^abstractionsSuffix.Length];
        }

        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentException("Target is required.", nameof(target));
        }

        return target;
    }

    private static string ResolveSourceRoot(string? sourceRoot)
    {
        if (string.IsNullOrWhiteSpace(sourceRoot))
        {
            sourceRoot = Directory.GetCurrentDirectory();
        }

        return Path.GetFullPath(sourceRoot);
    }

    private static string ResolveTemplateRoot()
    {
        var templateRoot = Path.Combine(AppContext.BaseDirectory, "Template");
        if (!Directory.Exists(templateRoot))
        {
            throw new DirectoryNotFoundException($"Template root not found: '{templateRoot}'.");
        }

        return templateRoot;
    }

    private static IReadOnlyList<string> ResolveModuleTemplateRoots(string templateRoot)
    {
        var templateRoots = new[]
        {
            Path.Combine(templateRoot, $"ZYC.Framework.Modules.{TemplateName}"),
            Path.Combine(templateRoot, $"ZYC.Framework.Modules.{TemplateName}.Abstractions")
        };

        foreach (var root in templateRoots)
        {
            if (!Directory.Exists(root))
            {
                throw new DirectoryNotFoundException($"Module template root not found: '{root}'.");
            }
        }

        return templateRoots;
    }

    private static string? ResolveSlnxPath(string sourceRoot, string? slnxPath)
    {
        if (string.IsNullOrWhiteSpace(slnxPath))
        {
            return null;
        }

        if (Path.IsPathRooted(slnxPath))
        {
            return Path.GetFullPath(slnxPath);
        }

        return Path.GetFullPath(Path.Combine(sourceRoot, slnxPath));
    }

    private static string GetShortName(string target)
    {
        return target.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                   .LastOrDefault()
               ?? target;
    }

    private static void EnsureTargetDoesNotExist(string sourceRoot, string target, bool overwrite)
    {
        if (overwrite)
        {
            return;
        }

        foreach (var targetDirectory in GetTargetDirectories(sourceRoot, target))
        {
            if (Directory.Exists(targetDirectory))
            {
                throw new IOException(
                    $"Target directory already exists: '{targetDirectory}'. Pass --overwrite to replace existing files.");
            }
        }
    }

    private static IEnumerable<string> GetTargetDirectories(string sourceRoot, string target)
    {
        yield return Path.Combine(sourceRoot, $"ZYC.Framework.Modules.{target}");
        yield return Path.Combine(sourceRoot, $"ZYC.Framework.Modules.{target}.Abstractions");
    }

    private static string GenerateFile(
        string templateRoot,
        string sourceRoot,
        string templateFilePath,
        string target,
        string shortName,
        bool overwrite)
    {
        var templateRelativePath = Path.GetRelativePath(templateRoot, templateFilePath);
        var targetRelativePath = ReplacePathFlags(templateRelativePath, target, shortName);
        var targetFilePath = Path.Combine(sourceRoot, targetRelativePath);
        var targetFolder = Path.GetDirectoryName(targetFilePath)
                           ?? throw new InvalidOperationException($"Cannot resolve target folder: '{targetFilePath}'.");

        Directory.CreateDirectory(targetFolder);

        if (!overwrite && File.Exists(targetFilePath))
        {
            throw new IOException(
                $"Target file already exists: '{targetFilePath}'. Pass --overwrite to replace existing files.");
        }

        var targetFileContent = ReplaceFileContent(templateFilePath, target, shortName);
        File.WriteAllText(targetFilePath, targetFileContent, Utf8BomEncoding);
        return targetFilePath;
    }

    private static string ReplacePathFlags(string path, string target, string shortName)
    {
        var result = path.Replace(
            $"ZYC.Framework.Modules.{TemplateName}.Abstractions",
            $"ZYC.Framework.Modules.{target}.Abstractions",
            StringComparison.Ordinal);
        result = result.Replace(
            $"ZYC.Framework.Modules.{TemplateName}",
            $"ZYC.Framework.Modules.{target}",
            StringComparison.Ordinal);

        result = result.Replace(TemplateName, shortName, StringComparison.Ordinal);
        result = result.Replace(TemplateName.ToLowerInvariant(), shortName.ToLowerInvariant(),
            StringComparison.Ordinal);
        return result;
    }

    private static string ReplaceFileContent(string file, string target, string shortName)
    {
        var content = File.ReadAllText(file);

        content = content.Replace(
            $"ZYC.Framework.Modules.{TemplateName}.Abstractions",
            $"ZYC.Framework.Modules.{target}.Abstractions",
            StringComparison.Ordinal);
        content = content.Replace(
            $"ZYC.Framework.Modules.{TemplateName}",
            $"ZYC.Framework.Modules.{target}",
            StringComparison.Ordinal);

        content = content.Replace(TemplateName, shortName, StringComparison.Ordinal);
        content = content.Replace(
            TemplateName.ToLowerInvariant(),
            shortName.ToLowerInvariant(),
            StringComparison.Ordinal);
        content = content.Replace("// ReSharper disable once CheckNamespace", "", StringComparison.Ordinal);

        return content;
    }

    private static void UpdateSlnx(string sourceRoot, string target, string slnxPath)
    {
        var projects = new[]
        {
            Path.Combine($"ZYC.Framework.Modules.{target}", $"ZYC.Framework.Modules.{target}.csproj"),
            Path.Combine($"ZYC.Framework.Modules.{target}.Abstractions",
                $"ZYC.Framework.Modules.{target}.Abstractions.csproj")
        };

        EnsureSlnxExists(slnxPath, sourceRoot);
        AddProjectsToSlnx(slnxPath, sourceRoot, projects);
    }

    private static void EnsureSlnxExists(string slnxPath, string sourceRoot)
    {
        if (File.Exists(slnxPath))
        {
            return;
        }

        var solutionDirectory = Path.GetDirectoryName(slnxPath)
                                ?? throw new InvalidOperationException(
                                    $"Cannot resolve solution directory: '{slnxPath}'.");
        Directory.CreateDirectory(solutionDirectory);

        var templateRoot = Path.Combine(sourceRoot, "ZYC.Framework.CLI", "Template");
        var csprojs = Directory.GetFiles(sourceRoot, "*.csproj", SearchOption.AllDirectories)
            .Where(p => !p.Contains(templateRoot, StringComparison.OrdinalIgnoreCase))
            .Select(p => ToSolutionRelativePath(slnxPath, p))
            .OrderBy(p => p, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var doc = new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement("Solution",
                new XElement("Configurations",
                    new XElement("Platform", new XAttribute("Name", "Any CPU")),
                    new XElement("Platform", new XAttribute("Name", "x64")),
                    new XElement("Platform", new XAttribute("Name", "x86"))
                ),
                csprojs.Select(p => new XElement("Project", new XAttribute("Path", p)))
            )
        );
        doc.Save(slnxPath);
    }

    private static void AddProjectsToSlnx(string slnxPath, string sourceRoot, IEnumerable<string> newProjects)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Load(slnxPath);
        }
        catch
        {
            EnsureSlnxExists(slnxPath, sourceRoot);
            doc = XDocument.Load(slnxPath);
        }

        var root = doc.Root;
        if (root == null || root.Name.LocalName != "Solution")
        {
            doc.RemoveNodes();
            root = new XElement("Solution");
            doc.Add(root);
        }

        var existing = doc.Descendants("Project")
            .Select(e => (string?)e.Attribute("Path") ?? string.Empty)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var modulesFolder = root.Elements("Folder")
                                .FirstOrDefault(e => string.Equals((string?)e.Attribute("Name"), "/Modules/",
                                    StringComparison.Ordinal))
                            ?? CreateFolder(root, "/Modules/");

        foreach (var project in newProjects)
        {
            var fullPath = Path.Combine(sourceRoot, project);
            var relativePath = ToSolutionRelativePath(slnxPath, fullPath);
            if (!existing.Contains(relativePath))
            {
                modulesFolder.Add(new XElement("Project", new XAttribute("Path", relativePath)));
                existing.Add(relativePath);
            }
        }

        doc.Save(slnxPath);
    }

    private static string ToSolutionRelativePath(string slnxPath, string filePath)
    {
        var solutionDirectory = Path.GetDirectoryName(slnxPath)
                                ?? throw new InvalidOperationException(
                                    $"Cannot resolve solution directory: '{slnxPath}'.");
        return Path.GetRelativePath(solutionDirectory, filePath).Replace('\\', '/');
    }

    private static XElement CreateFolder(XElement root, string name)
    {
        var folder = new XElement("Folder", new XAttribute("Name", name));
        root.Add(folder);
        return folder;
    }
}

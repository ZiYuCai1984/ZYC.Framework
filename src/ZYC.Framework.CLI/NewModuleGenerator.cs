using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ZYC.Framework.CLI;

public sealed class NewModuleGenerationOptions
{
    public string Target { get; init; } = string.Empty;

    public string? SourceRoot { get; init; }

    public bool Overwrite { get; init; }
}

public sealed class NewModuleGenerationResult
{
    public required string Target { get; init; }

    public required string SourceRoot { get; init; }

    public required IReadOnlyList<string> GeneratedFiles { get; init; }
}

public static class NewModuleGenerator
{
    private static readonly UTF8Encoding Utf8BomEncoding = new(true);

    public const string TemplateName = "Chronosynchronicity";

    public static NewModuleGenerationResult Generate(NewModuleGenerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var target = NormalizeTarget(options.Target);
        var sourceRoot = ResolveSourceRoot(options.SourceRoot);
        var templateRoot = ResolveTemplateRoot(sourceRoot);
        var shortName = GetShortName(target);

        EnsureTargetDoesNotExist(sourceRoot, target, options.Overwrite);

        var generatedFiles = Directory.GetFiles(templateRoot, "*.*", SearchOption.AllDirectories)
            .OrderBy(static path => path, StringComparer.OrdinalIgnoreCase)
            .Select(file => GenerateFile(templateRoot, sourceRoot, file, target, shortName, options.Overwrite))
            .ToList();

        UpdateSlnx(sourceRoot, target);

        return new NewModuleGenerationResult
        {
            Target = target,
            SourceRoot = sourceRoot,
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
        if (!string.IsNullOrWhiteSpace(sourceRoot))
        {
            return NormalizeSourceRoot(sourceRoot);
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
             directory != null;
             directory = directory.Parent)
        {
            foreach (var candidate in EnumerateSourceRootCandidates(directory.FullName))
            {
                if (seen.Add(candidate) && IsSourceRoot(candidate))
                {
                    return candidate;
                }
            }
        }

        throw new InvalidOperationException(
            "Cannot resolve the source root automatically. Pass --src-root with the repository root or src folder.");
    }

    private static string NormalizeSourceRoot(string sourceRoot)
    {
        foreach (var candidate in EnumerateSourceRootCandidates(Path.GetFullPath(sourceRoot)))
        {
            if (IsSourceRoot(candidate))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException(
            $"Cannot find 'ZYC.Framework.CLI\\Template' under '{sourceRoot}'. Pass the repository root or src folder.");
    }

    private static IEnumerable<string> EnumerateSourceRootCandidates(string path)
    {
        yield return path;
        yield return Path.Combine(path, "src");
    }

    private static bool IsSourceRoot(string path)
    {
        return Directory.Exists(Path.Combine(path, "ZYC.Framework.CLI", "Template"));
    }

    private static string ResolveTemplateRoot(string sourceRoot)
    {
        var templateRoot = Path.Combine(sourceRoot, "ZYC.Framework.CLI", "Template");
        if (!Directory.Exists(templateRoot))
        {
            throw new DirectoryNotFoundException($"Template root not found: '{templateRoot}'.");
        }

        return templateRoot;
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
        result = result.Replace(TemplateName.ToLowerInvariant(), shortName.ToLowerInvariant(), StringComparison.Ordinal);
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

    private static void UpdateSlnx(string sourceRoot, string target)
    {
        var slnxPath = Directory.GetFiles(sourceRoot, "*.slnx", SearchOption.TopDirectoryOnly).FirstOrDefault()
                       ?? Path.Combine(sourceRoot, "ZYC.Framework.slnx");

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

        var templateRoot = Path.Combine("ZYC.Framework.CLI", "Template");
        var csprojs = Directory.GetFiles(sourceRoot, "*.csproj", SearchOption.AllDirectories)
            .Where(p => !p.Contains(templateRoot, StringComparison.OrdinalIgnoreCase))
            .Select(p => ToSolutionRelativePath(sourceRoot, p))
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
            .FirstOrDefault(e => string.Equals((string?)e.Attribute("Name"), "/Modules/", StringComparison.Ordinal))
            ?? CreateFolder(root, "/Modules/");

        foreach (var project in newProjects)
        {
            var fullPath = Path.Combine(sourceRoot, project);
            var relativePath = ToSolutionRelativePath(sourceRoot, fullPath);
            if (!existing.Contains(relativePath))
            {
                modulesFolder.Add(new XElement("Project", new XAttribute("Path", relativePath)));
                existing.Add(relativePath);
            }
        }

        doc.Save(slnxPath);
    }

    private static string ToSolutionRelativePath(string sourceRoot, string filePath)
    {
        return Path.GetRelativePath(sourceRoot, filePath).Replace('\\', '/');
    }

    private static XElement CreateFolder(XElement root, string name)
    {
        var folder = new XElement("Folder", new XAttribute("Name", name));
        root.Add(folder);
        return folder;
    }
}


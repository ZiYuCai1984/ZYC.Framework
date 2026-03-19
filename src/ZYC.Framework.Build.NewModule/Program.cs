using System.IO;
using System.Text;
using System.Xml.Linq;

namespace ZYC.Framework.Build.NewModule;

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
            $"Cannot find 'ZYC.Framework.Build.NewModule\\Template' under '{sourceRoot}'. Pass the repository root or src folder.");
    }

    private static IEnumerable<string> EnumerateSourceRootCandidates(string path)
    {
        yield return path;
        yield return Path.Combine(path, "src");
    }

    private static bool IsSourceRoot(string path)
    {
        return Directory.Exists(Path.Combine(path, "ZYC.Framework.Build.NewModule", "Template"));
    }

    private static string ResolveTemplateRoot(string sourceRoot)
    {
        var templateRoot = Path.Combine(sourceRoot, "ZYC.Framework.Build.NewModule", "Template");
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

        var templateRoot = Path.Combine("ZYC.Framework.Build.NewModule", "Template");
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

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            if (ShouldShowHelp(args))
            {
                WriteHelp();
                return 0;
            }

            var options = ParseArguments(args);
            var result = NewModuleGenerator.Generate(options);

            Console.WriteLine($"Created module '{result.Target}'.");
            Console.WriteLine($"Source root: {result.SourceRoot}");

            foreach (var file in result.GeneratedFiles)
            {
                Console.WriteLine(file);
            }

            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            Console.Error.WriteLine("Use --help to view command usage.");
            return 1;
        }
    }

    private static bool ShouldShowHelp(IEnumerable<string> args)
    {
        return args.Any(arg => string.Equals(arg, "--help", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(arg, "-h", StringComparison.OrdinalIgnoreCase));
    }

    private static NewModuleGenerationOptions ParseArguments(IReadOnlyList<string> args)
    {
        string? target = null;
        string? sourceRoot = null;
        var overwrite = false;

        for (var index = 0; index < args.Count; index++)
        {
            var argument = args[index];
            switch (argument)
            {
                case "--target":
                case "-t":
                    target = ReadArgumentValue(args, ref index, argument);
                    break;
                case "--src-root":
                case "-s":
                    sourceRoot = ReadArgumentValue(args, ref index, argument);
                    break;
                case "--overwrite":
                case "-f":
                    overwrite = true;
                    break;
                default:
                    if (argument.StartsWith("-", StringComparison.Ordinal))
                    {
                        throw new ArgumentException($"Unknown argument '{argument}'.");
                    }

                    if (!string.IsNullOrWhiteSpace(target))
                    {
                        throw new ArgumentException("Target was provided multiple times.");
                    }

                    target = argument;
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(target))
        {
            throw new ArgumentException("Target is required. Pass --target <ModuleName>.");
        }

        return new NewModuleGenerationOptions
        {
            Target = target,
            SourceRoot = sourceRoot,
            Overwrite = overwrite
        };
    }

    private static string ReadArgumentValue(IReadOnlyList<string> args, ref int index, string argumentName)
    {
        var valueIndex = index + 1;
        if (valueIndex >= args.Count)
        {
            throw new ArgumentException($"Missing value for '{argumentName}'.");
        }

        index = valueIndex;
        return args[valueIndex];
    }

    private static void WriteHelp()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project .\\src\\ZYC.Framework.Build.NewModule -- --target <ModuleName> [--src-root <RepoRootOrSrc>] [--overwrite]");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project .\\src\\ZYC.Framework.Build.NewModule -- --target Blog --src-root .\\src");
        Console.WriteLine("  dotnet run --project .\\src\\ZYC.Framework.Build.NewModule -- Translator --src-root .");
    }
}
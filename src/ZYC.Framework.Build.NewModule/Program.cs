using System.Runtime.CompilerServices;
using System.Xml.Linq;
using ZYC.CoreToolkit;

namespace ZYC.Framework.Build.NewModule;

internal class Program
{
    private static string Flag => "Chronosynchronicity";

    private static string Target => "AAA";

    private static string DefaultTarget => "AAA";

    private static string ShortName
        => Target.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .LastOrDefault() ?? Target;

    private static void Main()
    {
        if (DefaultTarget == Target)
        {
            return;
        }

        var current = IOTools.GetCallerDirectoryPath();
        IOTools.SetCurrentDirectory(new DirectoryInfo(current).Parent!.FullName);

        var rootFolder = "ZYC.Framework.Build.NewModule\\Template\\";

        var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var filePath = StringTools.ReplaceOnce(file, rootFolder, "");
            var targetFileContent = ReplaceFileContent(file);
            var targetFilePath = ReplacePathFlags(filePath);

            var targetFolder = new FileInfo(targetFilePath).Directory!.FullName;
            IOTools.EnsureDirectoryExists(targetFolder);

            File.WriteAllText(targetFilePath, targetFileContent);
        }

        UpdateSlnx();

        RestoreTarget();
    }

    private static void RestoreTarget(
        [CallerFilePath] string callerFilePath = "")
    {
        var current = IOTools.GetCallerDirectoryPath();
        IOTools.SetCurrentDirectory(current);

        var content = File.ReadAllText(callerFilePath);
        content = content.Replace(
            $"private static string Target => \"{Target}\"",
            $"private static string Target => \"{DefaultTarget}\"");
        File.WriteAllText(callerFilePath, content);
    }

    private static string ReplacePathFlags(string path)
    {
        var result = path.Replace($"ZYC.Framework.Modules.{Flag}.Abstractions",
            $"ZYC.Framework.Modules.{Target}.Abstractions");
        result = result.Replace($"ZYC.Framework.Modules.{Flag}", $"ZYC.Framework.Modules.{Target}");

        result = result.Replace(Flag, ShortName, StringComparison.InvariantCulture);
        result = result.Replace(Flag.ToLowerInvariant(), ShortName.ToLowerInvariant(),
            StringComparison.InvariantCulture);
        return result;
    }

    private static string ReplaceFileContent(string file)
    {
        var content = File.ReadAllText(file);

        content = content.Replace($"ZYC.Framework.Modules.{Flag}.Abstractions",
            $"ZYC.Framework.Modules.{Target}.Abstractions", StringComparison.InvariantCulture);
        content = content.Replace($"ZYC.Framework.Modules.{Flag}", $"ZYC.Framework.Modules.{Target}",
            StringComparison.InvariantCulture);

        content = content.Replace(Flag, ShortName, StringComparison.InvariantCulture);

        content = content.Replace(Flag.ToLowerInvariant(), ShortName.ToLowerInvariant(),
            StringComparison.InvariantCulture);

        content = content.Replace("// ReSharper disable once CheckNamespace", "");

        return content;
    }

    private static void UpdateSlnx()
    {
        var srcRoot = Directory.GetCurrentDirectory();
        var slnxPath = Directory.GetFiles(srcRoot, "*.slnx", SearchOption.TopDirectoryOnly).FirstOrDefault()
                       ?? Path.Combine(srcRoot, "ZYC.Framework.slnx");

        var projects = new[]
        {
            Path.Combine($"ZYC.Framework.Modules.{Target}", $"ZYC.Framework.Modules.{Target}.csproj"),
            Path.Combine($"ZYC.Framework.Modules.{Target}.Abstractions",
                $"ZYC.Framework.Modules.{Target}.Abstractions.csproj")
        };

        EnsureSlnxExists(slnxPath, srcRoot);
        AddProjectsToSlnx(slnxPath, srcRoot, projects);
    }

    private static void EnsureSlnxExists(string slnxPath, string srcRoot)
    {
        if (File.Exists(slnxPath))
        {
            return;
        }

        var csprojs = Directory.GetFiles(srcRoot, "*.csproj", SearchOption.AllDirectories)
            .Where(p => !p.Contains("ZYC.Framework.Build.NewModule\\Template", StringComparison.OrdinalIgnoreCase))
            .Select(p =>
            {
                var rootUri = new Uri(srcRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);
                var fileUri = new Uri(p);
                return rootUri.MakeRelativeUri(fileUri).ToString();
            })
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

        private static void AddProjectsToSlnx(string slnxPath, string srcRoot, IEnumerable<string> newProjects)
    {
        XDocument doc;
        try
        {
            doc = XDocument.Load(slnxPath);
        }
        catch
        {
            EnsureSlnxExists(slnxPath, srcRoot);
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

        var rootUri = new Uri(srcRoot.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar);

        foreach (var p in newProjects)
        {
            var full = Path.Combine(srcRoot, p);
            var rel = rootUri.MakeRelativeUri(new Uri(full)).ToString();
            rel = rel.Replace('\\','/');
            if (!existing.Contains(rel))
            {
                modulesFolder.Add(new XElement("Project", new XAttribute("Path", rel)));
                existing.Add(rel);
            }
        }

        doc.Save(slnxPath);
    }

    private static XElement CreateFolder(XElement root, string name)
    {
        var folder = new XElement("Folder", new XAttribute("Name", name));
        root.Add(folder);
        return folder;
    }
}


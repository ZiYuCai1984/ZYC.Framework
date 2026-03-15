using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Build.Doc;

internal static class TemplateVariableResolver
{
    public static IReadOnlyDictionary<string, string> Resolve(DocumentationWorkspace workspace)
    {
        var variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        AddMsBuildProperties(variables, Path.Combine(workspace.SrcDirectory, "version.props"));
        AddMsBuildProperties(variables, Path.Combine(workspace.SrcDirectory, "nuget.props"));

        foreach (var pair in ProductInfo.Properties)
        {
            variables[pair.Key] = pair.Value;
        }

        variables["PackageId"] = ProductInfo.PackageId;
        variables["ProductName"] = ProductInfo.ProductName;
        variables["Version"] = ProductInfo.Version;
        variables["Description"] = ProductInfo.Description;
        variables["Author"] = ProductInfo.Author;
        variables["Copyright"] = ProductInfo.Copyright;
        variables["ProjectUrl"] = ProductInfoExtended.ProjectUrl;
        variables["Repository"] = ProductInfoExtended.Repository;
        variables["TargetFramework"] = ProductInfoExtended.TargetFramework;
        variables["ReleaseDate"] = DateTime.Now.ToString("yyyy-MM-dd");
        variables["RootDirectory"] = workspace.RootDirectory;
        variables["SrcDirectory"] = workspace.SrcDirectory;
        variables["DocsDirectory"] = workspace.DocsOutputDirectory;

        AddJsonOverrides(variables, workspace.VariablesFilePath);

        return variables;
    }

    private static void AddMsBuildProperties(IDictionary<string, string> variables, string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        var document = XDocument.Load(filePath);
        var propertyGroups = document.Root?.Elements("PropertyGroup") ?? Enumerable.Empty<XElement>();

        foreach (var propertyGroup in propertyGroups)
        {
            foreach (var property in propertyGroup.Elements())
            {
                if (property.HasElements)
                {
                    continue;
                }

                var value = property.Value.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    variables[property.Name.LocalName] = value;
                }
            }
        }
    }

    private static void AddJsonOverrides(IDictionary<string, string> variables, string filePath)
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        using var stream = File.OpenRead(filePath);
        var overrides = JsonSerializer.Deserialize<Dictionary<string, string>>(stream);
        if (overrides == null)
        {
            return;
        }

        foreach (var pair in overrides.Where(pair => !string.IsNullOrWhiteSpace(pair.Key)))
        {
            // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
            variables[pair.Key] = pair.Value ?? string.Empty;
        }
    }
}
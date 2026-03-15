using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ZYC.Framework.Build.Doc;

internal sealed class DocumentationBuilder
{
    private const string PrimaryReadmeTemplateName = "README.md";

    private static readonly UTF8Encoding Utf8BomEncoding = new(true);

    private static readonly string[] PreferredReadmeLocales = ["ja", "zh-CN", "zh-TW", "ko"];

    private static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".md",
        ".txt",
        ".json",
        ".yml",
        ".yaml",
        ".xml",
        ".props",
        ".targets",
        ".csproj",
        ".sln",
        ".slnx"
    };

    private static readonly Regex TemplateMetaCommentRegex = new(
        @"<!--doc-meta:[^>]*-->",
        RegexOptions.Multiline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly DocumentationWorkspace workspace;

    public DocumentationBuilder(DocumentationWorkspace workspace)
    {
        this.workspace = workspace;
    }

    public void Run()
    {
        EnsureRequiredTemplatesExist();

        var variables = TemplateVariableResolver.Resolve(workspace);
        var renderer = new TemplateRenderer(variables);
        var missingPlaceholders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var readmeFileCount = RenderReadmeTemplates(renderer, missingPlaceholders);
        var docFileCount = RenderTemplateFolder(
            workspace.DocsTemplateDirectory,
            workspace.DocsOutputDirectory,
            renderer,
            missingPlaceholders);

        Console.WriteLine($"README templates: {readmeFileCount}, docs templates: {docFileCount}.");

        if (missingPlaceholders.Count > 0)
        {
            Console.WriteLine(
                $"Unresolved placeholders: {string.Join(", ", missingPlaceholders.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))}");
        }
    }

    private void EnsureRequiredTemplatesExist()
    {
        if (!File.Exists(workspace.PrimaryReadmeTemplatePath))
        {
            throw new FileNotFoundException(
                $"README template not found: {workspace.PrimaryReadmeTemplatePath}");
        }
    }

    private int RenderReadmeTemplates(
        TemplateRenderer renderer,
        ISet<string> missingPlaceholders)
    {
        var templateContent = File.ReadAllText(workspace.PrimaryReadmeTemplatePath);
        if (LocalizedReadmeTemplate.ContainsBlocks(templateContent))
        {
            return RenderLocalizedReadmeTemplate(templateContent, renderer, missingPlaceholders);
        }

        var renderedContent = renderer.Render(templateContent, missingPlaceholders);
        WriteRenderedTextFile(
            Path.Combine(workspace.RootDirectory, PrimaryReadmeTemplateName),
            renderedContent);

        return 1;
    }

    private int RenderLocalizedReadmeTemplate(
        string templateContent,
        TemplateRenderer renderer,
        ISet<string> missingPlaceholders)
    {
        var template = LocalizedReadmeTemplate.Parse(templateContent);
        var locales = OrderReadmeLocales(template.Locales).ToArray();

        var renderedPrimaryContent = renderer.Render(template.Render(null), missingPlaceholders);
        WriteRenderedTextFile(
            Path.Combine(workspace.RootDirectory, PrimaryReadmeTemplateName),
            renderedPrimaryContent);

        foreach (var locale in locales)
        {
            var renderedLocalizedContent = renderer.Render(template.Render(locale), missingPlaceholders);
            WriteRenderedTextFile(
                Path.Combine(workspace.RootDirectory, GetReadmeOutputFileName(locale)),
                renderedLocalizedContent);
        }

        return locales.Length + 1;
    }

    private static IEnumerable<string> OrderReadmeLocales(IEnumerable<string> locales)
    {
        return locales.OrderBy(GetReadmeLocaleSortKey, StringComparer.OrdinalIgnoreCase)
            .ThenBy(locale => locale, StringComparer.OrdinalIgnoreCase);
    }

    private static string GetReadmeLocaleSortKey(string locale)
    {
        var index = Array.FindIndex(
            PreferredReadmeLocales,
            value => value.Equals(locale, StringComparison.OrdinalIgnoreCase));

        return index >= 0 ? $"{index:D2}-{locale}" : $"99-{locale}";
    }

    private static string GetReadmeOutputFileName(string locale)
    {
        return $"README.{locale}.md";
    }

    private static int RenderTemplateFolder(
        string templateRoot,
        string outputRoot,
        TemplateRenderer renderer,
        ISet<string> missingPlaceholders)
    {
        if (!Directory.Exists(templateRoot))
        {
            return 0;
        }

        var templateFiles = Directory
            .GetFiles(templateRoot, "*", SearchOption.AllDirectories)
            .Where(file => !ShouldSkipTemplateFile(file))
            .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var templateFile in templateFiles)
        {
            var relativePath = Path.GetRelativePath(templateRoot, templateFile);
            var renderedRelativePath = renderer.Render(relativePath, missingPlaceholders);
            var outputPath = Path.Combine(outputRoot, renderedRelativePath);
            var outputDirectory = Path.GetDirectoryName(outputPath);

            if (!string.IsNullOrWhiteSpace(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            if (IsTextTemplateFile(templateFile))
            {
                var content = File.ReadAllText(templateFile);
                var renderedContent = renderer.Render(content, missingPlaceholders);
                WriteRenderedTextFile(outputPath, renderedContent);
            }
            else
            {
                File.Copy(templateFile, outputPath, true);
            }
        }

        return templateFiles.Length;
    }

    private static void WriteRenderedTextFile(string path, string content)
    {
        var normalizedContent = StripTemplateMetaComments(content).TrimEnd() + Environment.NewLine;
        File.WriteAllText(path, normalizedContent, Utf8BomEncoding);
    }

    private static string StripTemplateMetaComments(string content)
    {
        return TemplateMetaCommentRegex.Replace(content, string.Empty);
    }

    private static bool ShouldSkipTemplateFile(string path)
    {
        var fileName = Path.GetFileName(path);
        return fileName.Equals(".gitkeep", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsTextTemplateFile(string path)
    {
        return TextExtensions.Contains(Path.GetExtension(path));
    }
}
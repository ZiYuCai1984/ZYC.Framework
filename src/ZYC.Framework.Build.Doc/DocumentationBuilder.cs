using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ZYC.Framework.Build.Doc;

internal sealed class DocumentationBuilder
{
    private const string PrimaryReadmeTemplateName = "README.md";

    private static readonly UTF8Encoding Utf8BomEncoding = new(true);

    private static readonly string[] PreferredLocales = ["ja", "zh-CN", "zh-TW", "ko"];

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
        if (LocalizedTemplate.ContainsBlocks(templateContent))
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
        var template = LocalizedTemplate.Parse(templateContent);
        var locales = OrderLocales(template.Locales).ToArray();

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

    private static IEnumerable<string> OrderLocales(IEnumerable<string> locales)
    {
        return locales.OrderBy(GetLocaleSortKey, StringComparer.OrdinalIgnoreCase)
            .ThenBy(locale => locale, StringComparer.OrdinalIgnoreCase);
    }

    private static string GetLocaleSortKey(string locale)
    {
        var index = Array.FindIndex(
            PreferredLocales,
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

        var renderedFileCount = 0;

        foreach (var templateFile in templateFiles)
        {
            var relativePath = Path.GetRelativePath(templateRoot, templateFile);
            var renderedRelativePath = renderer.Render(relativePath, missingPlaceholders);

            if (IsTextTemplateFile(templateFile))
            {
                var content = File.ReadAllText(templateFile);
                if (LocalizedTemplate.ContainsBlocks(content))
                {
                    renderedFileCount += RenderLocalizedTemplateFile(
                        content,
                        renderedRelativePath,
                        outputRoot,
                        renderer,
                        missingPlaceholders);
                    continue;
                }

                var outputPath = Path.Combine(outputRoot, renderedRelativePath);
                EnsureOutputDirectoryExists(outputPath);

                var renderedContent = renderer.Render(content, missingPlaceholders);
                WriteRenderedTextFile(outputPath, renderedContent);
                renderedFileCount++;
            }
            else
            {
                var outputPath = Path.Combine(outputRoot, renderedRelativePath);
                EnsureOutputDirectoryExists(outputPath);
                File.Copy(templateFile, outputPath, true);
                renderedFileCount++;
            }
        }

        return renderedFileCount;
    }

    private static int RenderLocalizedTemplateFile(
        string templateContent,
        string renderedRelativePath,
        string outputRoot,
        TemplateRenderer renderer,
        ISet<string> missingPlaceholders)
    {
        var template = LocalizedTemplate.Parse(templateContent);
        var locales = OrderLocales(template.Locales).ToArray();

        WriteRenderedLocalizedTextFile(
            Path.Combine(outputRoot, renderedRelativePath),
            template.Render(null),
            renderer,
            missingPlaceholders);

        foreach (var locale in locales)
        {
            var localizedRelativePath = GetLocalizedTemplateOutputFileName(renderedRelativePath, locale);
            WriteRenderedLocalizedTextFile(
                Path.Combine(outputRoot, localizedRelativePath),
                template.Render(locale),
                renderer,
                missingPlaceholders);
        }

        return locales.Length + 1;
    }

    private static void WriteRenderedLocalizedTextFile(
        string outputPath,
        string templateContent,
        TemplateRenderer renderer,
        ISet<string> missingPlaceholders)
    {
        EnsureOutputDirectoryExists(outputPath);

        var renderedContent = renderer.Render(templateContent, missingPlaceholders);
        WriteRenderedTextFile(outputPath, renderedContent);
    }

    private static string GetLocalizedTemplateOutputFileName(string relativePath, string locale)
    {
        var directory = Path.GetDirectoryName(relativePath);
        var extension = Path.GetExtension(relativePath);
        var fileName = Path.GetFileNameWithoutExtension(relativePath);
        var localizedFileName = string.IsNullOrEmpty(extension)
            ? $"{Path.GetFileName(relativePath)}.{locale}"
            : $"{fileName}.{locale}{extension}";

        return string.IsNullOrEmpty(directory)
            ? localizedFileName
            : Path.Combine(directory, localizedFileName);
    }

    private static void EnsureOutputDirectoryExists(string outputPath)
    {
        var outputDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrWhiteSpace(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
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

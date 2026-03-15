using System.IO;
using System.Runtime.CompilerServices;
using ZYC.Framework.Build.Utilities;

namespace ZYC.Framework.Build.Doc;

internal sealed class DocumentationWorkspace
{
    private DocumentationWorkspace(string projectDirectory)
    {
        ProjectDirectory = projectDirectory;
        TemplateDirectory = Path.Combine(ProjectDirectory, "Templates");
        ReadmeTemplateDirectory = Path.Combine(TemplateDirectory, "README");
        DocsTemplateDirectory = Path.Combine(TemplateDirectory, "docs");
        DocsOutputDirectory = Path.Combine(RootDirectory, "docs");
        VariablesFilePath = Path.Combine(TemplateDirectory, "variables.json");
        PrimaryReadmeTemplatePath = Path.Combine(ReadmeTemplateDirectory, "README.md");
    }

    public string ProjectDirectory { get; }

    public string SrcDirectory => BuildEnvironment.SrcFolder;

    public string RootDirectory => BuildEnvironment.RootFolder;

    public string TemplateDirectory { get; }

    public string ReadmeTemplateDirectory { get; }

    public string DocsTemplateDirectory { get; }

    public string DocsOutputDirectory { get; }

    public string VariablesFilePath { get; }

    public string PrimaryReadmeTemplatePath { get; }

    public static DocumentationWorkspace Create(
        [CallerFilePath] string callerFilePath = "")
    {
        var projectDirectory = Path.GetDirectoryName(callerFilePath)
                               ?? throw new InvalidOperationException("Unable to resolve build doc project directory.");

        return new DocumentationWorkspace(projectDirectory);
    }
}

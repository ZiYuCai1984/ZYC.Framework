namespace ZYC.Framework.Build.Doc;

internal static class Program
{
    public static void Main()
    {
        var workspace = DocumentationWorkspace.Create();
        var builder = new DocumentationBuilder(workspace);
        builder.Run();
    }
}

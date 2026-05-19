namespace ZYC.Framework.CLI;

public static class NewProjectCommandRunner
{
    public static int Run(
        NewProjectGenerationOptions options,
        Action<string> writeLine,
        Action<string> writeErrorLine,
        string? helpHint = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(writeLine);
        ArgumentNullException.ThrowIfNull(writeErrorLine);

        try
        {
            var result = NewProjectGenerator.Generate(options);

            writeLine($"Created project '{result.Name}'.");
            writeLine($"Template: {result.Template}");
            writeLine($"Output root: {result.OutputRoot}");

            foreach (var file in result.GeneratedFiles)
            {
                writeLine(file);
            }

            return 0;
        }
        catch (Exception exception)
        {
            writeErrorLine(exception.Message);
            if (!string.IsNullOrWhiteSpace(helpHint))
            {
                writeErrorLine(helpHint);
            }

            return 1;
        }
    }
}

namespace ZYC.Framework.CLI;

public static class NewModuleCommandRunner
{
    public static int Run(
        NewModuleGenerationOptions options,
        Action<string> writeLine,
        Action<string> writeErrorLine,
        string? helpHint = null)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(writeLine);
        ArgumentNullException.ThrowIfNull(writeErrorLine);

        try
        {
            var result = NewModuleGenerator.Generate(options);

            writeLine($"Created module '{result.Target}'.");
            writeLine($"Source root: {result.SourceRoot}");

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

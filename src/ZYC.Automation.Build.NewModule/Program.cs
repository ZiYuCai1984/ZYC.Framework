using System.Runtime.CompilerServices;
using ZYC.CoreToolkit;

namespace ZYC.Automation.Build.NewModule;

internal class Program
{
    private static string Flag => "Chronosynchronicity";

    private static string Target => "AAA";

    private static string DefaultTarget => "AAA";

    private static void Main()
    {
        if (DefaultTarget == Target)
        {
            return;
        }

        var current = IOTools.GetCallerDirectoryPath();
        IOTools.SetCurrentDirectory(new DirectoryInfo(current).Parent!.FullName);

        var rootFolder = "ZYC.Automation.Build.NewModule\\Template\\";

        var files = Directory.GetFiles(rootFolder, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var filePath = StringTools.ReplaceOnce(file, rootFolder, "");
            var targetFileContent = ReplaceFileContent(file);
            var targetFilePath = ReplaceFlag(filePath);

            var targetFolder = new FileInfo(targetFilePath).Directory!.FullName;
            IOTools.EnsureDirectoryExists(targetFolder);

            File.WriteAllText(targetFilePath, targetFileContent);
        }

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

    private static string ReplaceFlag(string content)
    {
        content = content.Replace(Flag, Target, StringComparison.InvariantCulture);
        content = content.Replace(Flag.ToLowerInvariant(), Target.ToLowerInvariant(),
            StringComparison.InvariantCulture);
        content = content.Replace("// ReSharper disable once CheckNamespace", "");

        return content;
    }


    private static string ReplaceFileContent(string file)
    {
        var content = File.ReadAllText(file);
        return ReplaceFlag(content);
    }
}
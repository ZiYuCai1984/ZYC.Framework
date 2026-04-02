using System.Diagnostics;
using System.IO;

namespace ZYC.Framework.Build.NewModule;

internal static class Program
{
    private static int Main(string[] args)
    {
        try
        {
            var cliProjectPath = ResolveCliProjectPath();

            var startInfo = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false
            };

            startInfo.ArgumentList.Add("run");
            startInfo.ArgumentList.Add("--project");
            startInfo.ArgumentList.Add(cliProjectPath);
            startInfo.ArgumentList.Add("--");
            startInfo.ArgumentList.Add("new-module");

            foreach (var argument in args)
            {
                startInfo.ArgumentList.Add(argument);
            }

            using var process = Process.Start(startInfo)
                                ?? throw new InvalidOperationException("Failed to start dotnet process.");

            process.WaitForExit();
            return process.ExitCode;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static string ResolveCliProjectPath()
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
             directory != null;
             directory = directory.Parent)
        {
            foreach (var candidate in EnumerateCliProjectCandidates(directory.FullName))
            {
                if (!seen.Add(candidate))
                {
                    continue;
                }

                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }
        }

        throw new FileNotFoundException(
            "Cannot find 'ZYC.Framework.CLI\\ZYC.Framework.CLI.csproj'. Run from repository root/src or use 'zyc new-module' directly.");
    }

    private static IEnumerable<string> EnumerateCliProjectCandidates(string path)
    {
        yield return Path.Combine(path, "src", "ZYC.Framework.CLI", "ZYC.Framework.CLI.csproj");
        yield return Path.Combine(path, "ZYC.Framework.CLI", "ZYC.Framework.CLI.csproj");
    }
}

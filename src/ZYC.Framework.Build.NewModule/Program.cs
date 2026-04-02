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
            var sourceRoot = ResolveSourceRoot(cliProjectPath, args);
            var slnxPath = ResolveDefaultSlnxPath(sourceRoot);

            var startInfo = new ProcessStartInfo("dotnet")
            {
                UseShellExecute = false
            };

            startInfo.ArgumentList.Add("run");
            startInfo.ArgumentList.Add("--project");
            startInfo.ArgumentList.Add(cliProjectPath);
            startInfo.ArgumentList.Add("--");
            startInfo.ArgumentList.Add("new-module");

            if (!HasSourceRootArgument(args))
            {
                startInfo.ArgumentList.Add("--src-root");
                startInfo.ArgumentList.Add(sourceRoot);
            }

            if (!HasSlnxArgument(args))
            {
                startInfo.ArgumentList.Add("--slnx");
                startInfo.ArgumentList.Add(slnxPath);
            }

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

    private static string ResolveSourceRoot(string cliProjectPath, IReadOnlyList<string> args)
    {
        var sourceRootFromArguments = TryGetSourceRootArgument(args);
        if (!string.IsNullOrWhiteSpace(sourceRootFromArguments))
        {
            return Path.GetFullPath(sourceRootFromArguments);
        }

        var projectDirectory = Path.GetDirectoryName(cliProjectPath)
                               ?? throw new InvalidOperationException($"Cannot resolve CLI project directory: '{cliProjectPath}'.");
        var sourceRoot = Directory.GetParent(projectDirectory)?.FullName;
        if (string.IsNullOrWhiteSpace(sourceRoot))
        {
            throw new InvalidOperationException($"Cannot resolve source root from '{cliProjectPath}'.");
        }

        return sourceRoot;
    }

    private static bool HasSourceRootArgument(IEnumerable<string> args)
    {
        return args.Any(arg =>
            string.Equals(arg, "--src-root", StringComparison.Ordinal)
            || string.Equals(arg, "-s", StringComparison.Ordinal)
            || arg.StartsWith("--src-root=", StringComparison.Ordinal)
            || arg.StartsWith("-s=", StringComparison.Ordinal));
    }

    private static string? TryGetSourceRootArgument(IReadOnlyList<string> args)
    {
        for (var index = 0; index < args.Count; index++)
        {
            var argument = args[index];
            if (string.Equals(argument, "--src-root", StringComparison.Ordinal)
                || string.Equals(argument, "-s", StringComparison.Ordinal))
            {
                var valueIndex = index + 1;
                if (valueIndex < args.Count)
                {
                    return args[valueIndex];
                }

                return null;
            }

            const string longPrefix = "--src-root=";
            if (argument.StartsWith(longPrefix, StringComparison.Ordinal))
            {
                return argument[longPrefix.Length..];
            }

            const string shortPrefix = "-s=";
            if (argument.StartsWith(shortPrefix, StringComparison.Ordinal))
            {
                return argument[shortPrefix.Length..];
            }
        }

        return null;
    }

    private static string ResolveDefaultSlnxPath(string sourceRoot)
    {
        return Directory.GetFiles(sourceRoot, "*.slnx", SearchOption.TopDirectoryOnly).FirstOrDefault()
               ?? Path.Combine(sourceRoot, "ZYC.Framework.slnx");
    }

    private static bool HasSlnxArgument(IEnumerable<string> args)
    {
        return args.Any(arg =>
            string.Equals(arg, "--slnx", StringComparison.Ordinal)
            || arg.StartsWith("--slnx=", StringComparison.Ordinal));
    }
}

using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Dotnet;

namespace __PROJECT_NAME__.Build;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        BuildEnvironment.UpdateVersionProps();

        var tempSlnFile = BuildEnvironment.TempSlnFile;

        try
        {
            IOTools.DeleteFileIfExists(tempSlnFile);

            var csprojFiles = BuildEnvironment.GetCsprojFilePaths(BuildEnvironment.RootFolder);
            ProjectTools.GenerateSln(csprojFiles, tempSlnFile);

            var exitCode = await CommandTools.ExecuteCommandAsync($"dotnet build \"{tempSlnFile}\" -c release");
            if (exitCode != 0)
            {
                return exitCode;
            }

            foreach (var csprojFile in csprojFiles)
            {
                await DotnetNuGetTools.PackProjectNoRestoreNoBuildAsync(
                    csprojFile,
                    BuildEnvironment.OutputPath,
                    BuildEnvironment.BuildVersion);
            }

#if PUSH_NUGET
            var nuGetUser = Environment.GetEnvironmentVariable("NUGET_USER");
            if (string.IsNullOrWhiteSpace(nuGetUser))
            {
                throw new InvalidOperationException("Set NUGET_USER to the NuGet account used for trusted publishing.");
            }

            var apiKey = await NuGetTrustedPublishingTools.GetApiKeyAsync(nuGetUser);
            await DotnetNuGetTools.PushNuGetAsync(
                BuildEnvironment.OutputPath,
                "https://api.nuget.org/v3/index.json",
                apiKey);
#endif

            return 0;
        }
        finally
        {
            IOTools.DeleteFileIfExists(tempSlnFile);
        }
    }
}

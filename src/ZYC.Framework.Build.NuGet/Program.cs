using System.IO;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Nuspec;
using ZYC.CoreToolkit.Dotnet;
using ZYC.Framework.Abstractions;
using ZYC.Framework.Modules.ApiReference.Abstractions;

namespace ZYC.Framework.Build.NuGet;

public class Program
{
    private static async Task Main()
    {
#if PUBLISH_NUGET_ORG
        var apiKey = Environment.GetEnvironmentVariable("NUGET_APIKEY");
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Ensure api key not be none !!");
        }
#endif

        var tempSlnFileName = "_temp.sln";

        try
        {
            await BuildSolutionAync(tempSlnFileName);

#if PUBLISH_NUGET_ORG
            await GenerateDocAsync();
#endif
            //await DotnetNuGetTools.PushLocalAsync(BuildEnvironment.SrcFolder);

#if PUBLISH_NUGET_ORG
        await DotnetNuGetTools.PushNuGetAsync(
            BuildEnvironment.SrcFolder,
            BuildEnvironment.NuGetPushSource,
            apiKey);
#endif
        }
        finally
        {
            IOTools.DeleteFileIfExists(tempSlnFileName);
        }
    }

    private static async Task GenerateDocAsync()
    {
        IOTools.SetCurrentDirectory(BuildEnvironment.RootFolder);

        await DotnetToolTools.InstallGlobalAsync("docfx");

        var docGenerateResult = await CommandTools.ExecuteCommandAsync("docfx docfx.json");
        if (docGenerateResult != 0)
        {
            throw new InvalidOperationException("Doc generate failed !!");
        }

        IOTools.CopyDirectory(
            Path.Combine(BuildEnvironment.RootFolder,"_site"),
            Path.Combine(BuildEnvironment.OutputPath, ApiReferenceModuleConstants.DocFolder));

        await PackProductAsync();
    }

    private static async Task BuildSolutionAync(string tempSlnFileName)
    {
        IOTools.SetCurrentDirectory(BuildEnvironment.SrcFolder);

        BuildEnvironment.UpdateVersionProps();

        var csprojFiles = GetCsprojFilePaths(BuildEnvironment.SrcFolder);


        await IOTools.ClearPathAsync(BuildEnvironment.SettingsDirectoryPath);


        IOTools.DeleteFileIfExists(tempSlnFileName);
        ProjectTools.GenerateSln(csprojFiles, tempSlnFileName);


        var buildResult = await CommandTools.ExecuteCommandAsync($"dotnet build {tempSlnFileName} -c release");
        if (buildResult != 0)
        {
            throw new InvalidOperationException("Build failed !!");
        }


        IOTools.DeleteDirectoryIfExists(BuildEnvironment.LogPath);
        IOTools.DeleteDirectoryIfExists(BuildEnvironment.ProductPackagePath);
        IOTools.DeleteDirectoryIfExists(BuildEnvironment.NuGetCachePath);

        var pendingRemoveFiles = Directory.GetFiles(
            BuildEnvironment.OutputPath, "*.*", SearchOption.AllDirectories).Where(f => f.EndsWith(".pdb") ||
            f.EndsWith(".xml") || f.EndsWith(".txt") || f.EndsWith(".endpoints.json")
            || f.EndsWith(".runtime.json")).ToArray();

        foreach (var file in pendingRemoveFiles)
        {
            File.Delete(file);
        }
    }


    private static string[] GetCsprojFilePaths(string projectRootFolder)
    {
        var allCsprojFiles = IOTools.GetAllCsprojFiles(projectRootFolder);

        var result = new List<string>();

        foreach (var file in allCsprojFiles)
        {
            var content = File.ReadAllText(file);
            if (content.Contains("<IgnoreFromPublish>true</IgnoreFromPublish>"))
            {
                continue;
            }

            result.Add(file);
        }

        return result.ToArray();
    }


    private static async Task PackProductAsync()
    {
        IOTools.SetCurrentDirectory(BuildEnvironment.SrcFolder);

        var package = GetProductPackage();

        var packageContent = XmlTools.Serialize(package);
        packageContent = StringTools.ReplaceOnce(packageContent, "<license />", "");
        packageContent = StringTools.ReplaceOnce(packageContent, "<licenseUrl />", "");

        await DotnetNuGetTools.PackNuGetByZipAsync(package, () =>
        {
            IOTools.CopyDirectory(
                $"{BuildEnvironment.BuildProjectPath}\\build",
                $"{BuildEnvironment.OutputPath}\\build");

            IOTools.CopyFile(
                BuildEnvironment.NuGetPropsPath,
                $"{BuildEnvironment.OutputPath}\\build\\nuget.props");

            IOTools.CopyFile(
                BuildEnvironment.NuGetTargetsPath,
                $"{BuildEnvironment.OutputPath}\\build\\nuget.targets");

            IOTools.CopyDirectory(
                $"{BuildEnvironment.BuildProjectPath}\\SharedSources",
                $"{BuildEnvironment.OutputPath}\\SharedSources");

            IOTools.CopyFile(
                BuildEnvironment.AppPngPath,
                $"{BuildEnvironment.OutputPath}\\app.png");
            IOTools.CopyFile(
                BuildEnvironment.NuGetREADMEPath,
                $"{BuildEnvironment.OutputPath}\\README.md");

            IOTools.CopyFiles(
                BuildEnvironment.OutputPath,
                BuildEnvironment.ProductPackagePath, false);

            File.WriteAllText(
                BuildEnvironment.ProductPackageNuspecPath,
                packageContent);
        });
    }

    private static Package GetProductPackage()
    {
        var group = new Group
        {
            TargetFramework = ProductInfoExtended.TargetFramework,
            Dependency =
            [
                // !WARNING When restoring packages from a local source, this section may cause some dependencies to be missing during restore.
                // Workarounds:
                // 1. Temporarily disable/recompile this step (comment out the regeneration).
                // 2. First restore all dependencies from nuget.org, then copy the restored
                //    packages into the local source before performing the local restore.
                new Dependency
                {
                    Id = "ZYC.CoreToolkit",
                    Version = BuildEnvironment.CoreToolkitVersion,
                    Exclude = "Build,Analyzers"
                },
                new Dependency
                {
                    Id = "ZYC.CoreToolkit.Extensions.Autofac",
                    Version = BuildEnvironment.CoreToolkitVersion,
                    Exclude = "Build,Analyzers"
                }
            ]
        };

        var dependencies = new Dependencies
        {
            Group = group
        };

        var metadata = new Metadata
        {
            Id = BuildEnvironment.ProductPackageId,
            Version = BuildEnvironment.BuildVersion,
            Authors = ProductInfo.Author,
            Copyright = ProductInfo.Copyright,
            Icon = "app.png",
            ProjectUrl = ProductInfoExtended.Repository,
            ReleaseNotes = GetPatchNote(),
            Description = ProductInfo.Description,
            Dependencies = dependencies
        };

        var package = new Package
        {
            XmlnsReplaceFlag = "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd",
            Metadata = metadata
        };

        return package;
    }

    private static string GetPatchNote()
    {
        var patchNote = File.ReadAllText(
            BuildEnvironment.PatchNotePath);

        patchNote = patchNote.Replace("$(Version)", ProductInfo.Version);
        patchNote = patchNote.Replace("$(ReleaseDate)", DateTime.Now.ToString("yyyy-MM-dd"));

        return patchNote;
    }
}
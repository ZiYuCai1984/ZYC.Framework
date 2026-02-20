using System.IO;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions.Nuspec;
using ZYC.CoreToolkit.Dotnet;
using ZYC.Framework.Abstractions;

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


        IOTools.SetCurrentDirectory(BuildEnvironment.RootFolder);

        BuildEnvironment.UpdateVersionProps();

        var csprojFiles = GetCsprojFilePaths(BuildEnvironment.RootFolder);


        await IOTools.ClearPathAsync(BuildEnvironment.OutputPath);


        var tempSlnFile = "_temp.sln";

        IOTools.DeleteFileIfExists(tempSlnFile);
        ProjectTools.GenerateSln(csprojFiles, tempSlnFile);


        var buildResult = await CommandTools.ExecuteCommandAsync($"dotnet build {tempSlnFile} -c release");
        if (buildResult != 0)
        {
            throw new InvalidOperationException("Build failed !!");
        }


        IOTools.DeleteDirectoryIfExists(BuildEnvironment.AlternatePath);
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

        await PackProductAsync();

        await DotnetNuGetTools.PushLocalAsync(BuildEnvironment.GetProjectRootFolderPath());


#if PUBLISH_NUGET_ORG
        await DotnetNuGetTools.PushNuGetAsync(
            BuildEnvironment.GetProjectRootFolderPath(),
            BuildEnvironment.NuGetPushSource,
            apiKey);
#endif

        IOTools.DeleteFileIfExists(tempSlnFile);
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
        IOTools.SetCurrentDirectory(BuildEnvironment.RootFolder);

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
                },
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
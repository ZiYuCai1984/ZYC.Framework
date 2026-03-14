using System.IO;
using ZYC.CoreToolkit;
using ZYC.CoreToolkit.Abstractions;
using ZYC.CoreToolkit.Dotnet;
using ProductInfo = ZYC.Framework.Abstractions.ProductInfo;

namespace ZYC.Framework.Build.InnoSetup;

internal class Program
{
    private static async Task Main()
    {
        IOTools.SetCurrentDirectoryToEntryScriptFileDirectory();

        IOTools.DeleteDirectoryIfExists("packages");

        var packageName = "Tools.InnoSetup";
        var version = "6.3.1";

        await DotnetNuGetTools.DownloadNuGetPackagesAsync(new NuGetPackage
        {
            Name = packageName,
            Version = version
        });

        var toolFolder = $"packages/{packageName}/{version}/tools";


        IOTools.CopyFile("../ZYC.Framework/app.ico", $"{toolFolder}/app.ico");
        IOTools.CopyDirectory("../_bin", $"{toolFolder}/_bin");
        IOTools.CopyFile("./app.iss", $"{toolFolder}/app.iss");

        IOTools.SetCurrentDirectory(toolFolder);

        var command =
            $"iscc.exe app.iss /DVersion=\"{ProductInfo.Version}\" /DCopyright=\"{ProductInfo.Copyright}\" /DAuthor=\"{ProductInfo.Author}\"";

        await CommandTools.ExecuteCommandAsync(command);

        IOTools.CopyFile(
            "./Output/ZYC.Framework.Setup.exe",
            $"{GetProjectSrcFolderPath()}/ZYC.Framework.Setup.{ProductInfo.Version}.exe");
    }

    public static string GetProjectSrcFolderPath()
    {
        var directoryPath = IOTools.GetCallerDirectoryPath();
        var directoryInfo = new DirectoryInfo(directoryPath);
        var path = directoryInfo.Parent!.FullName;
        return path;
    }
}
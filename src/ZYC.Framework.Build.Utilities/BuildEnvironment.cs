using System.IO;
using System.Text.RegularExpressions;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Build.Utilities;

// ReSharper disable InconsistentNaming
public static class BuildEnvironment
{
    public static string SetupFilePath = Path.Combine(SrcFolder, $"ZYC.Framework.Setup.{ProductInfo.Version}.exe");

    public static string RootFolder => GetProjectRootFolderPath();

    public static string DocFolder => Path.Combine(RootFolder, "docs\\");

    public static string SrcFolder => GetProjectSrcFolderPath();

    public static string AppRootDirectoryPath => Path.Combine(SrcFolder, "_bin\\");

    public static string OutputPath => Path.Combine(AppRootDirectoryPath, $"{ProductInfo.Version}\\");

    public static string ObfuscarConfigFile => $"{OutputPath}Obfuscar.xml";

    public static string ObfuscarPath => Path.Combine(OutputPath, "Obfuscar");

    public static string LogPath => Path.Combine(AppRootDirectoryPath, "logs");

    public static string ProductPackageId => ProductInfo.PackageId;

    public static string ProductPackagePath => Path.Combine(SrcFolder, ProductPackageId);

    public static string ProductPackageNuspecPath => $"{ProductPackagePath}\\{ProductPackageId}.nuspec";

    public static string NuGetCachePath => Path.Combine(OutputPath, ".cache");

    public static string RuntimesPath => Path.Combine(OutputPath, "runtimes");

    public static string RuntimesPath_win_arm64 => Path.Combine(RuntimesPath, "win-arm64");

    public static string RuntimesPath_win10_arm64 => Path.Combine(RuntimesPath, "win10-arm64");
    
    public static string RuntimesPath_win10_x86 => Path.Combine(RuntimesPath, "win10-x86");
    
    public static string RuntimesPath_win_x86 => Path.Combine(RuntimesPath, "win-x86");

    public static string BuildProjectPath => Path.Combine(SrcFolder, "ZYC.Framework.Build.NuGet");

    public static string PatchNotePath => $"{BuildProjectPath}\\PatchNote.md";

    public static string AppPngPath => $"{BuildProjectPath}\\app.png";

    public static string NuGetTargetsPath => Path.Combine(SrcFolder, "nuget.targets");

    public static string NuGetPropsPath => Path.Combine(SrcFolder, "nuget.props");

    public static string NuGetREADMEPath => Path.Combine(BuildProjectPath, "README.md");

    public static string GlobalJsonPath => Path.Combine(SrcFolder, "global.json");

    public static string BuildVersion => ProductInfo.Version;

    public static string VersionPropsPath => $"{SrcFolder}\\version.props";

    public static string NuGetPushSource => "https://api.nuget.org/v3/index.json";


    public static string CoreToolkitVersion => ReadVersionFromNuGetProps("V_ZYC_CoreToolkit");

    public static string AspireVersion => ReadVersionFromNuGetProps("V_Aspire");

    private static string GetProjectSrcFolderPath()
    {
        var directoryPath = IOTools.GetCallerDirectoryPath();
        var directoryInfo = new DirectoryInfo(directoryPath);
        var path = directoryInfo.Parent!.FullName;
        return path;
    }


    private static string GetProjectRootFolderPath()
    {
        var path = new DirectoryInfo(GetProjectSrcFolderPath()).Parent!.FullName;
        return path;
    }

    public static void UpdateVersionProps()
    {
        var props =
            "<Project>\r\n" +
            "  <PropertyGroup>\r\n" +
            $"    <Version>{BuildVersion}</Version>\r\n" +
            "  </PropertyGroup>\r\n" +
            "</Project>";

        File.WriteAllText(VersionPropsPath, props);
    }

    private static string ReadVersionFromNuGetProps(string propertyName)
    {
        var content = File.ReadAllText(NuGetPropsPath);
        var regex = new Regex(
            $"(?s)<{Regex.Escape(propertyName)}>\\s*([^<]+?)\\s*</{Regex.Escape(propertyName)}>");
        var result = regex.Match(content).Groups[1].Value.Trim();
        if (string.IsNullOrWhiteSpace(result))
        {
            throw new InvalidOperationException(
                $"Cannot find '{propertyName}' in '{NuGetPropsPath}'.");
        }

        return result;
    }
}
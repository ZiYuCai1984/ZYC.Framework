using System.IO;
using System.Text.RegularExpressions;
using ZYC.CoreToolkit;
using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Build.NuGet;

public static class BuildEnvironment
{
    public static string RootFolder => GetProjectRootFolderPath();

    public static string SrcFolder => GetProjectSrcFolderPath();

    public static string SettingsDirectoryPath => Path.Combine(SrcFolder, "_bin\\");

    public static string OutputPath => Path.Combine(SettingsDirectoryPath, $"{ProductInfo.Version}\\");

    public static string ObfuscarConfigFile => $"{OutputPath}Obfuscar.xml";

    public static string ObfuscarPath => Path.Combine(OutputPath, "Obfuscar");

    public static string LogPath => Path.Combine(SettingsDirectoryPath, "logs");

    public static string ProductPackageId => ProductInfo.PackageId;

    public static string ProductPackagePath => Path.Combine(SrcFolder, ProductPackageId);

    public static string ProductPackageNuspecPath => $"{ProductPackagePath}\\{ProductPackageId}.nuspec";

    public static string NuGetCachePath => Path.Combine(OutputPath, ".cache");

    public static string BuildProjectPath => Path.Combine(SrcFolder, "ZYC.Framework.Build.NuGet");

    public static string PatchNotePath => $"{BuildProjectPath}\\PatchNote.md";

    public static string AppPngPath => $"{BuildProjectPath}\\app.png";

    public static string NuGetTargetsPath => Path.Combine(SrcFolder, "nuget.targets");

    public static string NuGetPropsPath => Path.Combine(SrcFolder, "nuget.props");

    public static string NuGetREADMEPath => Path.Combine(BuildProjectPath, "README.md");

    public static string BuildVersion => ProductInfo.Version;

    public static string VersionPropsPath => $"{SrcFolder}\\version.props";

    public static string NuGetPushSource => "https://api.nuget.org/v3/index.json";


    public static string CoreToolkitVersion
    {
        get
        {
            var content = File.ReadAllText(NuGetPropsPath);
            var regex = new Regex("(?s)<V_ZYC_CoreToolkit>\\s*([^<]+?)\\s*</V_ZYC_CoreToolkit>");
            var result = regex.Match(content).Groups[1].Value;
            return result;
        }
    }

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
}
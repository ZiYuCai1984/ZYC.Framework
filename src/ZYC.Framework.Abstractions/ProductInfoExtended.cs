namespace ZYC.Framework.Abstractions;

/// <summary>
///     Contains extended metadata and configuration constants for the ZYC.Framework product.
///     This class serves as a central repository for executable names, project links, and build targets.
/// </summary>
public static class ProductInfoExtended
{
    /// <summary>
    ///     Gets the file name of the primary GUI application executable.
    /// </summary>
    public static string MainExeName => "ZYC.Framework.exe";

    /// <summary>
    ///     Gets the file name of the Command Line Interface (CLI) application executable.
    /// </summary>
    public static string CLIExeName => "ZYC.Framework.CLI.exe";

    /// <summary>
    ///     Gets the URL of the project's public landing page or documentation.
    /// </summary>
    public static string ProjectUrl => "https://github.com/ZiYuCai1984/ZYC.Framework";

    /// <summary>
    ///     Gets the source control repository URL for the project.
    /// </summary>
    public static string Repository => "https://github.com/ZiYuCai1984/ZYC.Framework";

    /// <summary>
    ///     Gets the target .NET version the framework is built for (e.g., .NET 10.0).
    /// </summary>
    public static string TargetFramework => "net10.0";

    /// <summary>
    ///     Gets the filename used to store metadata about NuGet module assets within the framework.
    /// </summary>
    public static string NuGetModuleAssetsJsonFile => "nuget.module.assets.json";
}
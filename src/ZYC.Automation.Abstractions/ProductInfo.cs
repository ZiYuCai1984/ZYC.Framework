namespace ZYC.Automation.Abstractions;

/// <summary>
///     Provides product-level metadata constants.
/// </summary>
public static class ProductInfo
{
    /// <summary>
    ///     Gets the package identifier.
    /// </summary>
    public static string PackageId => "ZYC.Automation.Alpha";

    /// <summary>
    ///     Gets the product name.
    /// </summary>
    public static string ProductName => "ZYC.Automation.Alpha";

    /// <summary>
    ///     Gets the URI scheme used by the product.
    /// </summary>
    public static string Scheme => "zyc";

    //!WARNING In order to adapt and release the version, the <Version> class cannot be used here

    /// <summary>
    ///     Gets the product version string.
    /// </summary>
    public static string Version => "1.0.7";

    /// <summary>
    ///     Gets the product description.
    /// </summary>
    public static string Description =>
        "A highly extensible .NET WPF framework featuring Aspire integration and seamless Blazor interoperability for modern hybrid applications.";

    /// <summary>
    ///     Gets the copyright notice.
    /// </summary>
    public static string Copyright => CoreToolkit.Abstractions.ProductInfo.Copyright;

    /// <summary>
    ///     Gets the product author.
    /// </summary>
    public static string Author => CoreToolkit.Abstractions.ProductInfo.Author;

    /// <summary>
    ///     Gets the main executable file name.
    /// </summary>
    public static string MainExeName => "ZYC.Automation.exe";

    /// <summary>
    ///     Gets the CLI executable file name.
    /// </summary>
    public static string CLIExeName => "ZYC.Automation.CLI.exe";

    public static string Repository => ProjectUrl;

    public static string ProjectUrl => "https://github.com/ZiYuCai1984/ZYC.Automation";

    public static string TargetFramework => "net10.0";

    public static string NuGetModuleAssetsJsonFile => "nuget.module.assets.json";
}
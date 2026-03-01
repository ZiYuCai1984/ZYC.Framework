using System.Collections.ObjectModel;
using System.Reflection;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Provides product-level metadata constants.
/// </summary>
public static class ProductInfo
{
    static ProductInfo()
    {
        var dict = typeof(ProductInfoExtended)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(p =>
                p.PropertyType == typeof(string) &&
                p.GetMethod is { IsPublic: true, IsStatic: true } &&
                p.GetIndexParameters().Length == 0)
            .OrderBy(p => p.Name, StringComparer.Ordinal)
            .ToDictionary(
                p => p.Name,
                p => (string?)p.GetValue(null) ?? string.Empty,
                StringComparer.Ordinal);

        Properties = new ReadOnlyDictionary<string, string>(dict);
    }

    /// <summary>
    ///     Gets the package identifier.
    /// </summary>
    public static string PackageId => "ZYC.Framework.Alpha";

    /// <summary>
    ///     Gets the product name.
    /// </summary>
    public static string ProductName => "ZYC.Framework.Alpha";

    /// <summary>
    ///     Gets the URI scheme used by the product.
    /// </summary>
    public static string Scheme => "zyc";

    //!WARNING In order to adapt and release the version, the <Version> class cannot be used here

    /// <summary>
    ///     Gets the product version string.
    /// </summary>
    public static string Version => "1.0.8";

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
    ///     Gets the dictionary of extended configuration properties for the product.
    /// </summary>
    public static IReadOnlyDictionary<string, string> Properties { get; }
}
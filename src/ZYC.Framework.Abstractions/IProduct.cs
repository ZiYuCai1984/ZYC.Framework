namespace ZYC.Framework.Abstractions;

/// <summary>
///     Defines the minimum metadata contract for a product or product update.
/// </summary>
/// <remarks>
///     Implementations can choose to provide additional fields (e.g., download URL, checksum, size),
///     but these properties represent the core information typically shown to users.
/// </remarks>
public interface IProduct
{
    /// <summary>
    ///     Gets the package identifier of the product.
    /// </summary>
    string PackageId { get; }

    /// <summary>
    ///     Gets the version string of the product or update.
    /// </summary>
    string Version { get; }

    /// <summary>
    ///     Gets a human-readable description of the product or update.
    /// </summary>
    string Description { get; }

    /// <summary>
    ///     Gets the patch notes / release notes for this update.
    /// </summary>
    /// <remarks>
    ///     The default implementation returns <see cref="string.Empty" />.
    ///     Implementations may override to provide actual patch notes.
    /// </remarks>
    string PatchNote => string.Empty;

    /// <summary>
    ///     Gets the copyright notice for the product.
    /// </summary>
    string Copyright { get; }

    /// <summary>
    ///     Gets the author or publisher of the product/update.
    /// </summary>
    string Author { get; }
}
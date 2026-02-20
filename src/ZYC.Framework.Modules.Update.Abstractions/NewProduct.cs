using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Update.Abstractions;

/// <summary>
///     Represents a discovered product update that can be downloaded and applied.
/// </summary>
/// <remarks>
///     This type is an immutable snapshot of update metadata (package id, version, author, description, patch note).
/// </remarks>
public class NewProduct : IProduct
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="NewProduct" /> class.
    /// </summary>
    /// <param name="packageId">
    ///     The package identifier of the product.
    /// </param>
    /// <param name="version">
    ///     The version string of the update.
    /// </param>
    /// <param name="author">
    ///     The author or publisher of the update.
    /// </param>
    /// <param name="description">
    ///     A human-readable description of the update or product.
    /// </param>
    /// <param name="patchNote">
    ///     The patch notes / release notes for this update.
    /// </param>
    public NewProduct(
        string packageId,
        string version,
        string author,
        string description,
        string patchNote)
    {
        Author = author;
        Description = description;
        PatchNote = patchNote;
        PackageId = packageId;
        Version = version;
    }

    /// <summary>
    ///     Gets the package identifier of the product.
    /// </summary>
    public string PackageId { get; }

    /// <summary>
    ///     Gets the version string of the update.
    /// </summary>
    public string Version { get; }

    /// <summary>
    ///     Gets the copyright notice for the product.
    /// </summary>
    /// <remarks>
    ///     This implementation currently returns an empty string.
    /// </remarks>
    public string Copyright => "";

    /// <summary>
    ///     Gets the author or publisher of the update.
    /// </summary>
    public string Author { get; }

    /// <summary>
    ///     Gets a human-readable description of the update or product.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the patch notes / release notes for this update.
    /// </summary>
    public string PatchNote { get; }
}
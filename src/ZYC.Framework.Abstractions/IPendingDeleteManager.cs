using ZYC.CoreToolkit.Abstractions.Attributes;

namespace ZYC.Framework.Abstractions;

/// <summary>
///     Tracks files pending deletion.
/// </summary>
[TempCode]
public interface IPendingDeleteManager
{
    /// <summary>
    ///     Adds a file to the pending deletion list.
    /// </summary>
    /// <param name="fileName">The file name to track.</param>
    void Add(string fileName);

    /// <summary>
    ///     Checks whether a file is pending deletion.
    /// </summary>
    /// <param name="fileName">The file name to check.</param>
    /// <returns>True if pending; otherwise, false.</returns>
    bool Contains(string fileName);

    /// <summary>
    ///     Gets the tracked file names.
    /// </summary>
    /// <returns>The tracked file names.</returns>
    string[] GetFiles();
}
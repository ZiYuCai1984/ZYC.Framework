namespace ZYC.Framework.Abstractions;

/// <summary>
///     Represents a provider for the root grid instance.
/// </summary>
public interface IRootGrid
{
    /// <summary>
    ///     Gets the root grid instance.
    /// </summary>
    /// <returns>The root grid instance.</returns>
    object GetRootGrid();
}
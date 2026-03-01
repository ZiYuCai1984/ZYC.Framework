namespace ZYC.Framework.Abstractions;

/// <summary>
///     Represents a provider for the root AdornerLayer instance.
/// </summary>
public interface IRootAdornerLayer
{
    /// <summary>
    ///     Gets the root AdornerLayer instance.
    /// </summary>
    /// <returns>The root AdornerLayer instance.</returns>
    object GetRootAdornerLayer();
}
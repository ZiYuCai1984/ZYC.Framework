namespace ZYC.Framework.Abstractions.Overlay;

/// <summary>
///     Manages the lifecycle and display of overlays within the application.
/// </summary>
public interface IOverlayManager
{
    /// <summary>
    ///     Displays an overlay on top of a specified target.
    /// </summary>
    /// <param name="target">The UI element or object that the overlay should be attached to or cover.</param>
    /// <param name="passThrough">Optional data or parameters to be passed to the overlay instance for initialization.</param>
    /// <returns>An <see cref="IOverlay" /> instance, which can be disposed to hide or remove the overlay.</returns>
    IOverlay Show(object target, object? passThrough = null);
}
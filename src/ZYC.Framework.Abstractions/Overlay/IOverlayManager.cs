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
    /// <param name="passThrough">An optional additional UI element or object that should remain interactive beneath the overlay.</param>
    /// <returns>An <see cref="IOverlay" /> instance, which can be disposed to hide or remove the overlay.</returns>
    IOverlay Show(object target, object? passThrough = null);

    /// <summary>
    ///     Displays a single overlay with all specified target areas remaining visible and interactive.
    /// </summary>
    /// <param name="targets">The UI elements or objects whose areas should remain visible and interactive.</param>
    /// <param name="passThrough">An optional additional UI element or object that should remain interactive beneath the overlay.</param>
    /// <returns>An <see cref="IOverlay" /> instance, which can be disposed to hide or remove the entire overlay.</returns>
    IOverlay Show(object[] targets, object? passThrough = null);
}

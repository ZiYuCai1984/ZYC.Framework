namespace ZYC.Framework.Abstractions.StatusBar;

/// <summary>
/// Defines the visual contract for the status bar view component.
/// Provides methods to access runtime layout properties.
/// </summary>
public interface IStatusBarView
{
    /// <summary>
    /// Gets the rendered height of the status bar in device-independent units.
    /// This is useful for adjusting adjacent UI elements or overlays.
    /// </summary>
    /// <returns>The actual calculated height of the view.</returns>
    double GetActualHeight();
}
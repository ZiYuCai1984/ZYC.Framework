namespace ZYC.Framework.Abstractions.WindowTitle;

/// <summary>
///     Defines a contract for items that extend or provide custom content within a window's title bar.
/// </summary>
public interface IWindowTitleExtendItem
{
    /// <summary>
    ///     Gets the visual element or view object to be displayed in the title bar area.
    /// </summary>
    object View { get; }
}
namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     Defines unique anchor identifiers for the "File" main menu category.
///     These strings combine a priority prefix with the member name for consistent sorting and identification.
/// </summary>
public static class FileMainMenuAnchors
{
    /// <summary> Anchor for the "Open" menu action. </summary>
    public static string Open => $"010_{nameof(Open)}";

    /// <summary> Anchor for the "Exit" menu action. </summary>
    public static string Exit => $"020_{nameof(Exit)}";
}

/// <summary>
///     Defines unique anchor identifiers for the "About" main menu category.
/// </summary>
public static class AboutMainMenuAnchors
{
    /// <summary> Anchor for the "Update" menu action. </summary>
    public static string Update => $"010_{nameof(Update)}";

    /// <summary> Anchor for the "About" (Information/Version) menu action. </summary>
    public static string About => $"020_{nameof(About)}";
}
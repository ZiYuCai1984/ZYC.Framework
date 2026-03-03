namespace ZYC.Framework.Abstractions.MainMenu;

/// <summary>
///     Provides a set of predefined priority constants for standard top-level menu categories
///     to ensure consistent ordering.
/// </summary>
public static class MainMenuPriority
{
    /// <summary> Priority for the "File" menu (typically the first item). </summary>
    public static int File => 010;

    /// <summary> Priority for the "View" menu. </summary>
    public static int View => 030;

    /// <summary> Priority for the "Tools" menu. </summary>
    public static int Tools => 050;

    /// <summary> Priority for the "Extensions" menu. </summary>
    public static int Extensions => 070;

    /// <summary> Priority for the "About" or "Help" menu. </summary>
    public static int About => 090;
}
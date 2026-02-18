using ZYC.Automation.Abstractions.MCP;

namespace ZYC.Automation.Abstractions;

/// <summary>
///     Represents a main window abstraction for the application.
/// </summary>
[ExposeToMCP(InvokeOnUIThread = true)]
public partial interface IMainWindow
{
    /// <summary>
    ///     Sets the window state.
    /// </summary>
    /// <param name="windowState">The desired window state.</param>
    void SetWindowState(WindowState windowState);

    /// <summary>
    ///     Gets the current window state.
    /// </summary>
    /// <returns>The current window state.</returns>
    WindowState GetWindowState();

    /// <summary>
    ///     Sets whether the window is always on top.
    /// </summary>
    /// <param name="topmost">Whether the window should be topmost.</param>
    void SetTopmost(bool topmost);

    /// <summary>
    ///     Shows the window.
    /// </summary>
    void Show();

    /// <summary>
    ///     Hides the window.
    /// </summary>
    void Hide();

    /// <summary>
    ///     Sets the window width.
    /// </summary>
    /// <param name="width">The window width in pixels.</param>
    void SetWindowWidth(int width);

    /// <summary>
    ///     Sets the window height.
    /// </summary>
    /// <param name="height">The window height in pixels.</param>
    void SetWindowHeight(int height);

    /// <summary>
    ///     Sets whether the window is visible in the taskbar.
    /// </summary>
    /// <param name="value">Whether to show the window in the taskbar.</param>
    void SetShowInTaskbar(bool value);

    /// <summary>
    ///     Gets whether the window is visible in the taskbar.
    /// </summary>
    /// <returns>True if visible in the taskbar; otherwise, false.</returns>
    bool GetShowInTaskbar();

    /// <summary>
    ///     Gets the native window handle.
    /// </summary>
    /// <returns>The native window handle.</returns>
    IntPtr GetWindowHandle();

    /// <summary>
    ///     Sets whether the window is frozen for interaction.
    /// </summary>
    /// <param name="value">Whether to freeze the window.</param>
    void SetIsFrozen(bool value);
}

public partial interface IMainWindow
{
    /// <summary>
    ///     Initiates a window drag move.
    /// </summary>
    [MCPIgnore]
    void DragMove();

    /// <summary>
    ///     Initializes the main window content.
    /// </summary>
    /// <param name="content">The content to display.</param>
    [MCPIgnore]
    void InitContent(object content);

    /// <summary>
    ///     Gets the underlying main window instance.
    /// </summary>
    /// <returns>The main window instance.</returns>
    [MCPIgnore]
    object GetMainWindow();
}
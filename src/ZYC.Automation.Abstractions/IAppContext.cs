using ZYC.Automation.Abstractions.MCP;

namespace ZYC.Automation.Abstractions;

/// <summary>
///     Provides application-level context services.
/// </summary>
[ExposeToMCP]
public partial interface IAppContext
{
    /// <summary>
    ///     Gets the current working directory.
    /// </summary>
    /// <returns>The current directory path.</returns>
    string GetCurrentDirectory();

    /// <summary>
    ///     Gets the system temporary directory path.
    /// </summary>
    /// <returns>The temporary directory path.</returns>
    string GetTempPath();

    /// <summary>
    ///     Gets the main application directory.
    /// </summary>
    /// <returns>The main application directory path.</returns>
    string GetMainAppDirectory();

    /// <summary>
    ///     Gets the alternate application directory.
    /// </summary>
    /// <returns>The alternate application directory path.</returns>
    string GetAlternateAppDirectory();

    /// <summary>
    ///     Gets the current process file name.
    /// </summary>
    /// <returns>The process file name.</returns>
    string GetProcessFileName();

    /// <summary>
    ///     Gets the default WebView2 user data folder.
    /// </summary>
    /// <returns>The WebView2 user data folder path.</returns>
    string GetDefaultWebView2UserDataFolder();

    /// <summary>
    ///     Gets whether the current app is running in the alternate location.
    /// </summary>
    /// <returns>True if running as alternate; otherwise, false.</returns>
    bool IsSelfAlternate();

    /// <summary>
    ///     Saves all configuration data.
    /// </summary>
    void SaveAllConfig();

    /// <summary>
    ///     Saves all state data.
    /// </summary>
    void SaveAllState();

    /// <summary>
    ///     Gets whether the current app is the main instance.
    /// </summary>
    /// <returns>True if running as main; otherwise, false.</returns>
    bool IsSelfMain()
    {
        return !IsSelfAlternate();
    }

    /// <summary>
    ///     Switches the startup target between main and alternate.
    /// </summary>
    void SwitchStartupTarget();

    /// <summary>
    ///     Gets the argument string used to launch the application.
    /// </summary>
    /// <returns>The argument string.</returns>
    string GetArgumentString();

    /// <summary>
    ///     Exits the process using the standard application shutdown flow.
    /// </summary>
    [ExposeToMCP(RequiresUIThread = true)]
    void ExitProcess();

    /// <summary>
    ///     !WARNING Call Environment.Exit(0) and not invoke App.OnExit event !!
    /// </summary>
    void FocusExitProcess();

    /// <summary>
    ///     Gets the current startup target.
    /// </summary>
    /// <returns>The current startup target.</returns>
    StartupTarget GetCurrentStartupTarget()
    {
        if (IsSelfMain())
        {
            return StartupTarget.Main;
        }

        return StartupTarget.Alternate;
    }
}

public partial interface IAppContext
{
    /// <summary>
    ///     Gets the UI synchronization context.
    /// </summary>
    /// <returns>The UI synchronization context.</returns>
    [MCPIgnore]
    SynchronizationContext GetUISynchronizationContext();

    /// <summary>
    ///     Executes an action on the UI thread.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    [MCPIgnore]
    void InvokeOnUIThread(Action action);

    /// <summary>
    ///     Executes an asynchronous action on the UI thread.
    /// </summary>
    /// <param name="func">The async action to execute.</param>
    /// <returns>A task that completes when the action finishes.</returns>
    [MCPIgnore]
    Task InvokeOnUIThreadAsync(Func<Task> func);

    /// <summary>
    ///     Executes an asynchronous function on the UI thread.
    /// </summary>
    /// <typeparam name="T">The result type.</typeparam>
    /// <param name="func">The async function to execute.</param>
    /// <returns>A task that returns the function result.</returns>
    [MCPIgnore]
    Task<T> InvokeOnUIThreadAsync<T>(Func<Task<T>> func);

    /// <summary>
    ///     Occurs when the application is exiting.
    /// </summary>
    event EventHandler? Exit;

    /// <summary>
    ///     Occurs when the application is about to exit.
    /// </summary>
    event EventHandler<CancelEventArgsEx>? Exiting;
}
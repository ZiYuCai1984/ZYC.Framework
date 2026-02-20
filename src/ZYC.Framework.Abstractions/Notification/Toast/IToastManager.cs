namespace ZYC.Framework.Abstractions.Notification.Toast;

/// <summary>
///     Defines the contract for managing and displaying toast notifications.
/// </summary>
public interface IToastManager
{
    /// <summary>
    ///     Displays a toast notification of a specific type.
    /// </summary>
    /// <typeparam name="T">The type of toast to display, must implement IToast.</typeparam>
    void Prompt<T>() where T : IToast;

    /// <summary>
    ///     Displays a toast notification of a specific type with custom parameters.
    /// </summary>
    /// <typeparam name="T">The type of toast to display, must implement IToast.</typeparam>
    /// <param name="objects">Arguments used to initialize or format the toast content.</param>
    void Prompt<T>(params object[] objects) where T : IToast;

    /// <summary>
    ///     Displays a predefined toast indicating that content has been successfully copied.
    /// </summary>
    void PromptCopied();

    /// <summary>
    ///     Displays a toast notification containing error details from an exception.
    /// </summary>
    /// <param name="exception">The exception to be reported to the user.</param>
    void PromptException(Exception exception);

    /// <summary>
    ///     Displays a toast notification based on a specific message data structure.
    /// </summary>
    /// <param name="toastMessage">The message object containing toast configuration and text.</param>
    void PromptMessage(ToastMessage toastMessage);
}
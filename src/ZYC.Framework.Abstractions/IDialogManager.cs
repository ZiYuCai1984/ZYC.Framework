namespace ZYC.Framework.Abstractions;

/// <summary>
///     Manages application dialogs.
/// </summary>
public interface IDialogManager
{
    /// <summary>
    ///     Shows a non-modal dialog window.
    /// </summary>
    /// <typeparam name="T">The dialog type to resolve and show.</typeparam>
    void Show<T>() where T : IDialog;

    /// <summary>
    ///     Shows a non-modal dialog window with resolver parameters.
    /// </summary>
    /// <typeparam name="T">The dialog type to resolve and show.</typeparam>
    /// <param name="parameters">The resolver-specific parameters used to construct the dialog.</param>
    void Show<T>(params object[] parameters) where T : IDialog;
}

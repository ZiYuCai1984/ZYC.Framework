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
}

namespace ZYC.Framework.Abstractions;

/// <summary>
///     !WARNING It will be covered by WebView2 window, etc.
/// </summary>
public interface IDialogManager
{
    /// <summary>
    ///     Shows an input dialog and returns the entered value.
    /// </summary>
    /// <param name="content">The dialog content.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="localization">Whether to localize the text.</param>
    /// <returns>The input string.</returns>
    Task<string> ShowInputDialogAsync(string content, string caption, bool localization = true);

    /// <summary>
    ///     Shows a message dialog.
    /// </summary>
    /// <param name="content">The dialog content.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="buttonText">The button text.</param>
    /// <param name="localization">Whether to localize the text.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ShowMessageDialogAsync(string content, string caption, string buttonText, bool localization = true);
}
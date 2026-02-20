using ZYC.Framework.Abstractions;

namespace ZYC.Framework.Modules.Settings.Abstractions;

// ReSharper disable once InconsistentNaming
/// <summary>
///     Provides dialog extensions for settings navigation prompts.
/// </summary>
public static class IDialogManagerEx
{
    /// <summary>
    ///     Shows a dialog prompting the user to navigate to the settings page.
    /// </summary>
    /// <param name="dialogManager">The dialog manager.</param>
    /// <param name="content">The dialog content.</param>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="buttonText">The action button text.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static Task ShowNavigateToSettingsPageDialogAsync(
        this IDialogManager dialogManager,
        string content,
        string caption = "Prompt",
        string buttonText = "Go to settings page")
    {
        return dialogManager.ShowMessageDialogAsync(content, caption, buttonText);
    }
}